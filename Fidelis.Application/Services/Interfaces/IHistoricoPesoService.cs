using Fidelis.Application.Common;
using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IHistoricoPesoService
{
    PagedResult<HistoricoPesoResponse> GetAll(HistoricoPesoQueryParameters query);
    HistoricoPesoResponse? GetById(int id);
    HistoricoPesoCriadoResponse Create(HistoricoPesoRequest request);
    HistoricoPesoResponse? Update(int id, HistoricoPesoRequest request);
    HistoricoPesoResponse? Patch(int id, HistoricoPesoPatchRequest request);
    bool Delete(int id);
    AvaliacaoPesoResponse? GetAvaliacao(int petId);
}
