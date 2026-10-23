using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IVeterinarioService
{
    PagedResult<VeterinarioResponse> GetAll(VeterinarioQueryParameters query);
    VeterinarioResponse? GetById(int id);
    VeterinarioResponse Create(VeterinarioRequest request);
    VeterinarioResponse? Update(int id, VeterinarioRequest request);
    VeterinarioResponse? Patch(int id, VeterinarioPatchRequest request);
    bool Delete(int id);
}