using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IPrescricaoService
{
    PagedResult<PrescricaoResponse> GetAll(PrescricaoQueryParameters query);
    PrescricaoResponse? GetById(int id);
    PrescricaoResponse Create(PrescricaoRequest request);
    PrescricaoResponse? Update(int id, PrescricaoRequest request);
    PrescricaoResponse? Patch(int id, PrescricaoPatchRequest request);
    bool Delete(int id);
}
