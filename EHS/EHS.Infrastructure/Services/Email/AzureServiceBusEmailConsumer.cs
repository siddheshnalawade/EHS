using Azure.Messaging.ServiceBus;
using EHS.Application.DTOs;
using EHS.Domain.Settings;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Polly;
using Polly.Retry;

namespace EHS.Infrastructure.Services.Email
{
    public class AzureServiceBusEmailConsumer : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AzureServiceBusEmailConsumer> _logger;

        public AzureServiceBusEmailConsumer(IServiceProvider serviceProvider, IOptions<EmailSettings> settings, ILogger<AzureServiceBusEmailConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _client = new ServiceBusClient(settings.Value.ServiceBusConnectionString);
            _processor = _client.CreateProcessor(settings.Value.EmailQueueName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false, // We will complete manually after success
                MaxConcurrentCalls = 2
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("Azure Service Bus Email Consumer Started.");

            // Wait until cancelled
            try 
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch(TaskCanceledException) { }

            await _processor.StopProcessingAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            EmailRequest? emailRequest;
            
            try 
            {
                emailRequest = JsonSerializer.Deserialize<EmailRequest>(body);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize email request.");
                await args.DeadLetterMessageAsync(args.Message, "DeserializationError", ex.Message);
                return;
            }

            if(emailRequest == null) 
            {
                await args.CompleteMessageAsync(args.Message);
                return;   
            }

            try 
            {
                // Use a Scope for injection as FluentEmail might be scoped
                using var scope = _serviceProvider.CreateScope();
                var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();

                // Define Retry Policy using Polly
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning($"Email send failed. Retry {retryCount}.");
                        });

                await retryPolicy.ExecuteAsync(async () =>
                {
                    var email = fluentEmail
                        .To(emailRequest.ToEmail)
                        .Subject(emailRequest.Subject);

                    if (!string.IsNullOrEmpty(emailRequest.TemplateName))
                    {
                        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", emailRequest.TemplateName);
                         if (File.Exists(templatePath))
                        {
                                await email.UsingTemplateFromFile(templatePath, emailRequest.TemplateModel).SendAsync();
                        }
                        else
                        {
                            string msg = $"Email template not found at {templatePath}.";
                             email.Body(emailRequest.Body ?? msg, isHtml: true);
                            await email.SendAsync();
                        }
                    }
                    else
                    {
                        email.Body(emailRequest.Body, isHtml: true);
                        await email.SendAsync();
                    }
                });

                // If success:
                await args.CompleteMessageAsync(args.Message);
                _logger.LogInformation($"Successfully sent email to {emailRequest.ToEmail} and removed from queue.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailRequest.ToEmail}. Message will be abandoned/dead-lettered.");
                // Abandoning puts it back in queue immediately. DeadLetter sends it to DLQ. 
                // Since we already retried 3 times internally, we should probably DeadLetter it or Abandon it for a later retry by Service Bus (delivery count).
                // Let's rely on ServiceBus max delivery count.
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Message handler encountered an exception");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
             await _processor.CloseAsync(cancellationToken);
             await _client.DisposeAsync();
             await base.StopAsync(cancellationToken);
        }
    }
}
