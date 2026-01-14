using EHS.Application.DTOs;

namespace EHS.Application.Interfaces
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }
}
