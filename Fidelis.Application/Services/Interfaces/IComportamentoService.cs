using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IComportamentoService
{
    PagedResult<ComportamentoResponse> GetAll(ComportamentoQueryParameters query);
    ComportamentoResponse? GetById(int id);
    ComportamentoResponse Create(ComportamentoRequest request);
    ComportamentoResponse? Update(int id, ComportamentoRequest request);
    ComportamentoResponse? Patch(int id, ComportamentoPatchRequest request);
    bool Delete(int id);
}
