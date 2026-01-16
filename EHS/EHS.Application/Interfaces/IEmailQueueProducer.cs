using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    public interface IEmailQueueProducer
    {
        Task PublishEmailAsync(EmailRequest emailRequest);
    }
}
