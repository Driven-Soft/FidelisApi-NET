using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IVacinacaoService
{
    List<VacinacaoResponse> GetAll();
    VacinacaoResponse? GetById(int id);
    VacinacaoResponse Create(VacinacaoRequest request);
    VacinacaoResponse? Update(int id, VacinacaoRequest request);
    VacinacaoResponse? Patch(int id, VacinacaoPatchRequest request);
    bool Delete(int id);
}
