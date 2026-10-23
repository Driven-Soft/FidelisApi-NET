using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IRecomendacaoService
{
    PagedResult<RecomendacaoResponse> GetAll(RecomendacaoQueryParameters query);
    RecomendacaoResponse? GetById(int id);
    RecomendacaoResponse Create(RecomendacaoRequest request);
    RecomendacaoResponse? Update(int id, RecomendacaoRequest request);
    RecomendacaoResponse? Patch(int id, RecomendacaoPatchRequest request);
    bool Delete(int id);
}
