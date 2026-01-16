using EHS.Application.DTOs;
using EHS.Domain.Settings;
using EHS.Infrastructure.Services.Email;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace EHS.Infrastructure.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly EmailChannel _emailChannel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(
            EmailChannel emailChannel,
            IServiceProvider serviceProvider,
            ILogger<EmailBackgroundService> logger)
        {
            _emailChannel = emailChannel;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email Background Service started.");

            await foreach (var request in _emailChannel.ReadAllAsync(stoppingToken))
            {
                try
                {
                    // Define Retry Policy
                    var retryPolicy = Policy
                        .Handle<Exception>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            (exception, timeSpan, retryCount, context) =>
                            {
                                _logger.LogWarning($"Email failed. Retry {retryCount} in {timeSpan.TotalSeconds}s. Error: {exception.Message}");
                            });

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                        // var emailSettings = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value; // Not strictly needed inside if not re-configuring, but scope is fresh.

                        var email = fluentEmail
                            .To(request.ToEmail)
                            .Subject(request.Subject);

                        if (!string.IsNullOrEmpty(request.TemplateName))
                        {
                            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", request.TemplateName);
                            // _logger.LogInformation($"Attempting to load email template from: {templatePath}"); // Reduce verbosity
                            
                            if (File.Exists(templatePath))
                            {
                                    await email.UsingTemplateFromFile(templatePath, request.TemplateModel).SendAsync(stoppingToken);
                            }
                            else
                            {
                                string msg = $"Email template not found at {templatePath}.";
                                _logger.LogWarning(msg);
                                email.Body(request.Body ?? msg, isHtml: true);
                                await email.SendAsync(stoppingToken);
                            }
                        }
                        else
                        {
                            email.Body(request.Body, isHtml: true);
                            await email.SendAsync(stoppingToken);
                        }

                        _logger.LogInformation($"Email sent to {request.ToEmail}");
                    });
                }
                catch (Exception ex)
                {
                    // If all retries fail:
                    _logger.LogError(ex, $"FATAL: Error sending email to {request.ToEmail} after retries.");
                }
            }
        }
    }
}
