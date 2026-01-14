using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Infrastructure.Services.Email;

namespace EHS.Infrastructure.Services
{
    public class EmailService : ISendEmailService
    {
        private readonly EmailChannel _emailChannel;

        public EmailService(EmailChannel emailChannel)
        {
            _emailChannel = emailChannel;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            await _emailChannel.AddEmailAsync(request);
        }
    }
}
