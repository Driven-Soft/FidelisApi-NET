using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IVeterinarioService
{
    List<VeterinarioResponse> GetAll();
    VeterinarioResponse? GetById(int id);
    VeterinarioResponse Create(VeterinarioRequest request);
    VeterinarioResponse? Update(int id, VeterinarioRequest request);
    VeterinarioResponse? Patch(int id, VeterinarioPatchRequest request);
    bool Delete(int id);
}