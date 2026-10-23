using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.Interfaces.Repositories;

public interface IAuditoriaRepository
{
    PagedResult<LogAuditoria> GetPaged(AuditoriaQueryParameters parameters);
    LogAuditoria? GetById(string id);
    void Add(LogAuditoria log);
}
