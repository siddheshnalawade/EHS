using Azure.Messaging.ServiceBus;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EHS.Infrastructure.Services.Email
{
    public class AzureServiceBusEmailProducer : IEmailQueueProducer, IAsyncDisposable
    {
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusClient _client;
        private readonly ILogger<AzureServiceBusEmailProducer> _logger;

        public AzureServiceBusEmailProducer(IOptions<EmailSettings> settings, ILogger<AzureServiceBusEmailProducer> logger)
        {
            _logger = logger;
            _client = new ServiceBusClient(settings.Value.ServiceBusConnectionString);
            _sender = _client.CreateSender(settings.Value.EmailQueueName);
        }

        public async Task PublishEmailAsync(EmailRequest emailRequest)
        {
            try 
            {
                var jsonString = JsonSerializer.Serialize(emailRequest);
                var message = new ServiceBusMessage(jsonString);
                
                await _sender.SendMessageAsync(message);
                _logger.LogInformation($"Queued email for {emailRequest.ToEmail} to Azure Service Bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish email to Service Bus.");
                throw; // Rethrow to let caller know enqueue failed
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
