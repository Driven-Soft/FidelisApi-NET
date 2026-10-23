using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IVacinacaoService
{
    PagedResult<VacinacaoResponse> GetAll(VacinacaoQueryParameters query);
    VacinacaoResponse? GetById(int id);
    VacinacaoResponse Create(VacinacaoRequest request);
    VacinacaoResponse? Update(int id, VacinacaoRequest request);
    VacinacaoResponse? Patch(int id, VacinacaoPatchRequest request);
    bool Delete(int id);
}
