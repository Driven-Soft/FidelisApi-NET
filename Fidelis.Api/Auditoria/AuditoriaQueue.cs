using System.Threading.Channels;
using Fidelis.Domain.Entities;

namespace Fidelis.Api.Auditoria;

public class AuditoriaQueue : IAuditoriaQueue
{
    private const int Capacity = 1000;

    private readonly Channel<LogAuditoria> _channel = Channel.CreateBounded<LogAuditoria>(
        new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        });

    public bool Enqueue(LogAuditoria log) => _channel.Writer.TryWrite(log);

    public IAsyncEnumerable<LogAuditoria> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
