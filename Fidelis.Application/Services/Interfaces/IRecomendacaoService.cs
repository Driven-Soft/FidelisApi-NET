using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IRecomendacaoService
{
    List<RecomendacaoResponse> GetAll();
    RecomendacaoResponse? GetById(int id);
    RecomendacaoResponse Create(RecomendacaoRequest request);
    RecomendacaoResponse? Update(int id, RecomendacaoRequest request);
    RecomendacaoResponse? Patch(int id, RecomendacaoPatchRequest request);
    bool Delete(int id);
}
