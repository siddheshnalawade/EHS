using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    public interface ISmsQueueProducer
    {
        Task PublishSmsAsync(SmsRequest smsRequest);
    }
}
