using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IVermifugacaoService
{
    PagedResult<VermifugacaoResponse> GetAll(VermifugacaoQueryParameters query);
    VermifugacaoResponse? GetById(int id);
    VermifugacaoResponse Create(VermifugacaoRequest request);
    VermifugacaoResponse? Update(int id, VermifugacaoRequest request);
    VermifugacaoResponse? Patch(int id, VermifugacaoPatchRequest request);
    bool Delete(int id);
}
