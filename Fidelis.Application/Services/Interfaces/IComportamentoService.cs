using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IComportamentoService
{
    List<ComportamentoResponse> GetAll();
    ComportamentoResponse? GetById(int id);
    ComportamentoResponse Create(ComportamentoRequest request);
    ComportamentoResponse? Update(int id, ComportamentoRequest request);
    ComportamentoResponse? Patch(int id, ComportamentoPatchRequest request);
    bool Delete(int id);
}
