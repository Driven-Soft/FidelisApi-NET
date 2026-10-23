using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IPetService
{
    PagedResult<PetResponse> GetAll(PetQueryParameters query);
    PetResponse? GetById(int id);
    PetResponse Create(PetRequest request);
    PetResponse? Update(int id, PetRequest request);
    PetResponse? Patch(int id, PetPatchRequest request);
    bool Delete(int id);
}