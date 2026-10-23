using Fidelis.Domain.Entities;

namespace Fidelis.Api.Auditoria;

public interface IAuditoriaQueue
{
    bool Enqueue(LogAuditoria log);
    IAsyncEnumerable<LogAuditoria> ReadAllAsync(CancellationToken cancellationToken);
}
