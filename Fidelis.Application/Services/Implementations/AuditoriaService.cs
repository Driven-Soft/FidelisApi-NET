using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.Services.Implementations;

public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _repository;

    public AuditoriaService(IAuditoriaRepository repository)
    {
        _repository = repository;
    }

    public PagedResult<AuditoriaResponse> GetAll(AuditoriaQueryParameters query)
    {
        return _repository
            .GetPaged(query)
            .Map(log => new AuditoriaResponse(log));
    }

    public AuditoriaResponse? GetById(string id)
    {
        var log = _repository.GetById(id);
        return log is null ? null : new AuditoriaResponse(log);
    }

    public void Registrar(LogAuditoria log)
    {
        ArgumentNullException.ThrowIfNull(log);
        _repository.Add(log);
    }
}
