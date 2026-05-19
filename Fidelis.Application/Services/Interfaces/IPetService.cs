using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IPetService
{
    List<PetResponse> GetAll();
    PetResponse? GetById(int id);
    PetResponse Create(PetRequest request);
    PetResponse? Update(int id, PetRequest request);
    PetResponse? Patch(int id, PetPatchRequest request);
    bool Delete(int id);
}