using Azure.Messaging.ServiceBus;
using EHS.Application.DTOs;
using EHS.Application.Interfaces; // To access logic? Actually we need to call logic here.
using EHS.Domain.Settings; // Needed for Settings
using Microsoft.Extensions.DependencyInjection; // Needed for IServiceProvider
using Microsoft.Extensions.Hosting; // Needed for BackgroundService
using Microsoft.Extensions.Logging; // Needed for ILogger
using Microsoft.Extensions.Options; // Needed for Options
using System.Text.Json; // Needed for serialization
using Polly; // Needed for Polly
using Polly.Retry; // Needed for Polly
using EHS.Infrastructure.Services; // Namespace for SmsService (which is logic, not producer) - Wait, we need abstract logic for SENDING.

// The architecture is:
// Controller/Service -> Producer -> Queue
// Worker -> Consumer -> Logic
// Previously SmsService was both logic and sender.
// I need to separate the "Sending Logic" (Twilio API call) from the "Queue Producer" (SmsService wrapping Queue).
// But for now, I will use a simple internal logic method or keep using old code but wrapped.

namespace EHS.Infrastructure.Services.Sms
{
    public class AzureServiceBusSmsConsumer : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AzureServiceBusSmsConsumer> _logger;

        public AzureServiceBusSmsConsumer(IServiceProvider serviceProvider, IOptions<SmsSettings> settings, ILogger<AzureServiceBusSmsConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _client = new ServiceBusClient(settings.Value.ServiceBusConnectionString);
            _processor = _client.CreateProcessor(settings.Value.SmsQueueName, new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 2
            });
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("Azure Service Bus SMS Consumer Started.");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException) { }

            await _processor.StopProcessingAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
             string body = args.Message.Body.ToString();
            SmsRequest? smsRequest;
            
            try 
            {
                smsRequest = JsonSerializer.Deserialize<SmsRequest>(body);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize SMS request.");
                await args.DeadLetterMessageAsync(args.Message, "DeserializationError", ex.Message);
                return;
            }

            if(smsRequest == null) 
            {
                await args.CompleteMessageAsync(args.Message);
                return;   
            }


                // Real Logic using Azure Communication Services
                using var scope = _serviceProvider.CreateScope();
                var settings = scope.ServiceProvider.GetRequiredService<IOptions<SmsSettings>>().Value;

                if (!settings.Enabled)
                {
                    await args.CompleteMessageAsync(args.Message);
                    return;
                }

                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning($"SMS send failed. Retry {retryCount}. Error: {exception.Message}");
                        });

                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Initialize Client (In prod, better to inject singleton client, but this is fine for worker)
                    var smsClient = new Azure.Communication.Sms.SmsClient(settings.CommunicationServiceConnectionString);

                    // Send SMS
                    await smsClient.SendAsync(
                        from: settings.FromNumber, // Must be your purchased Azure Phone Number
                        to: smsRequest.ToPhoneNumber,
                        message: smsRequest.Message
                    );
                    
                    _logger.LogInformation($"[SMS SENT] To: {smsRequest.ToPhoneNumber}");
                });

                await args.CompleteMessageAsync(args.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to {smsRequest.ToPhoneNumber}.");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "SMS handler exception");
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
