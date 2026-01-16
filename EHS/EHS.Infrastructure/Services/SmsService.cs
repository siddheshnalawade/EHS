using EHS.Application.Interfaces;
using EHS.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EHS.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsQueueProducer _queueProducer;

        public SmsService(ISmsQueueProducer queueProducer)
        {
            _queueProducer = queueProducer;
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            await _queueProducer.PublishSmsAsync(new EHS.Application.DTOs.SmsRequest 
            { 
                ToPhoneNumber = toPhoneNumber, 
                Message = message 
            });
        }
    }
}
