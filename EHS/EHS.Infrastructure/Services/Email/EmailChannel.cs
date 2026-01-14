using EHS.Application.DTOs;
using System.Threading.Channels;

namespace EHS.Infrastructure.Services.Email
{
    public class EmailChannel
    {
        private readonly Channel<EmailRequest> _channel;

        public EmailChannel()
        {
            // Unbounded channel for simplicity, but could be bounded for backpressure
            _channel = Channel.CreateUnbounded<EmailRequest>();
        }

        public async Task AddEmailAsync(EmailRequest emailRequest, CancellationToken ct = default)
        {
            await _channel.Writer.WriteAsync(emailRequest, ct);
        }

        public IAsyncEnumerable<EmailRequest> ReadAllAsync(CancellationToken ct = default)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
