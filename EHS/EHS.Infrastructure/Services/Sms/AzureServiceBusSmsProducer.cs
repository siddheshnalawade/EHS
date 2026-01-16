using Azure.Messaging.ServiceBus;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EHS.Infrastructure.Services.Sms
{
    public class AzureServiceBusSmsProducer : ISmsQueueProducer, IAsyncDisposable
    {
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusClient _client;
        private readonly ILogger<AzureServiceBusSmsProducer> _logger;

        public AzureServiceBusSmsProducer(IOptions<SmsSettings> settings, ILogger<AzureServiceBusSmsProducer> logger)
        {
            _logger = logger;
            _client = new ServiceBusClient(settings.Value.ServiceBusConnectionString);
            _sender = _client.CreateSender(settings.Value.SmsQueueName);
        }

        public async Task PublishSmsAsync(SmsRequest smsRequest)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(smsRequest);
                var message = new ServiceBusMessage(jsonString);

                await _sender.SendMessageAsync(message);
                _logger.LogInformation($"Queued SMS for {smsRequest.ToPhoneNumber} to Azure Service Bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish SMS to Service Bus.");
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
