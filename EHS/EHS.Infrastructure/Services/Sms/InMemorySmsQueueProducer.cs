using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Infrastructure.Services.Sms;

namespace EHS.Infrastructure.Services
{
    public class InMemorySmsQueueProducer : ISmsQueueProducer
    {
        private readonly SmsChannel _channel;

        public InMemorySmsQueueProducer(SmsChannel channel)
        {
            _channel = channel;
        }

        public async Task PublishSmsAsync(SmsRequest smsRequest)
        {
            await _channel.AddSmsAsync(smsRequest);
        }
    }
}
