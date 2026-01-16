using EHS.Application.DTOs;
using System.Threading.Channels;

namespace EHS.Infrastructure.Services.Sms
{
    public class SmsChannel
    {
        private readonly Channel<SmsRequest> _channel;

        public SmsChannel()
        {
            _channel = Channel.CreateUnbounded<SmsRequest>();
        }

        public async Task AddSmsAsync(SmsRequest smsRequest, CancellationToken ct = default)
        {
            await _channel.Writer.WriteAsync(smsRequest, ct);
        }

        public IAsyncEnumerable<SmsRequest> ReadAllAsync(CancellationToken ct = default)
        {
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
