using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IRegistroDiarioService
{
    PagedResult<RegistroDiarioResponse> GetAll(RegistroDiarioQueryParameters query);
    RegistroDiarioResponse? GetById(string id);
    RegistroDiarioResponse Create(RegistroDiarioRequest request);
    RegistroDiarioResponse? Update(string id, RegistroDiarioUpdateRequest request);
    RegistroDiarioResponse? Patch(string id, RegistroDiarioPatchRequest request);
    bool Delete(string id);
}
