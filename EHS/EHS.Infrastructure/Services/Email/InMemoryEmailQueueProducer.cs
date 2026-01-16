using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Infrastructure.Services.Email;

namespace EHS.Infrastructure.Services
{
    public class InMemoryEmailQueueProducer : IEmailQueueProducer
    {
        private readonly EmailChannel _channel;

        public InMemoryEmailQueueProducer(EmailChannel channel)
        {
            _channel = channel;
        }

        public async Task PublishEmailAsync(EmailRequest emailRequest)
        {
            await _channel.AddEmailAsync(emailRequest);
        }
    }
}
