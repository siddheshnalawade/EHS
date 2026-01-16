using EHS.Application.DTOs;
using EHS.Infrastructure.Services.Sms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EHS.Domain.Settings;

namespace EHS.Infrastructure.Services
{
    public class SmsBackgroundService : BackgroundService
    {
        private readonly SmsChannel _channel;
        private readonly ILogger<SmsBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SmsBackgroundService(
            SmsChannel channel, 
            ILogger<SmsBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var request in _channel.ReadAllAsync(stoppingToken))
            {
                try
                {
                     // Simulate Sending for InMemory
                     _logger.LogInformation($"[In-Memory SMS Worker] Processed SMS to {request.ToPhoneNumber}: {request.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing SMS in background.");
                }
            }
        }
    }
}
