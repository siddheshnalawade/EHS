using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Infrastructure.Services.Email;

namespace EHS.Infrastructure.Services
{
    public class EmailService : ISendEmailService
    {
        private readonly IEmailQueueProducer _emailQueue;

        public EmailService(IEmailQueueProducer emailQueue)
        {
            _emailQueue = emailQueue;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            await _emailQueue.PublishEmailAsync(request);
        }
    }
}
