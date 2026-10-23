using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.Interfaces.Repositories;

public interface IRegistroDiarioRepository
{
    PagedResult<RegistroDiario> GetPaged(RegistroDiarioQueryParameters parameters);
    RegistroDiario? GetById(string id);
    void Add(RegistroDiario registro);
    bool Replace(RegistroDiario registro);
    bool Delete(string id);
}
