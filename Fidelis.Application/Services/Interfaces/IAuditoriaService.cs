using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.Services.Interfaces;

public interface IAuditoriaService
{
    PagedResult<AuditoriaResponse> GetAll(AuditoriaQueryParameters query);
    AuditoriaResponse? GetById(string id);
    void Registrar(LogAuditoria log);
}
