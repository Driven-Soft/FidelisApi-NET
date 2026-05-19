using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IHistoricoPesoService
{
    List<HistoricoPesoResponse> GetAll();
    HistoricoPesoResponse? GetById(int id);
    HistoricoPesoResponse Create(HistoricoPesoRequest request);
    HistoricoPesoResponse? Update(int id, HistoricoPesoRequest request);
    HistoricoPesoResponse? Patch(int id, HistoricoPesoPatchRequest request);
    bool Delete(int id);
}
