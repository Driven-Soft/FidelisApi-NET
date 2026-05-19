using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class HistoricoPesoService : IHistoricoPesoService
{
    private readonly IHistoricoPesoRepository _repository;

    public HistoricoPesoService(IHistoricoPesoRepository repository)
    {
        _repository = repository;
    }

    public List<HistoricoPesoResponse> GetAll()
    {
        var historicos = _repository.GetAll();
        return historicos.Select(historico => new HistoricoPesoResponse(historico)).ToList();
    }

    public HistoricoPesoResponse? GetById(int id)
    {
        var historico = _repository.GetById(id);
        return historico is null ? null : new HistoricoPesoResponse(historico);
    }

    public HistoricoPesoResponse Create(HistoricoPesoRequest request)
    {
        var historico = request.ToDomain();
        _repository.Add(historico);
        _repository.SaveChanges();
        return new HistoricoPesoResponse(historico);
    }

    public HistoricoPesoResponse? Update(int id, HistoricoPesoRequest request)
    {
        var historico = _repository.GetById(id);
        if (historico is null)
            return null;

        historico.UpdatePeso(request.PesoKg);
        _repository.Update(historico);
        _repository.SaveChanges();
        return new HistoricoPesoResponse(historico);
    }

    public HistoricoPesoResponse? Patch(int id, HistoricoPesoPatchRequest request)
    {
        var historico = _repository.GetById(id);
        if (historico is null)
            return null;

        if (request.PesoKg.HasValue)
            historico.UpdatePeso(request.PesoKg.Value);

        _repository.Update(historico);
        _repository.SaveChanges();
        return new HistoricoPesoResponse(historico);
    }

    public bool Delete(int id)
    {
        var historico = _repository.GetById(id);
        if (historico is null)
            return false;

        _repository.Delete(historico);
        _repository.SaveChanges();
        return true;
    }
}
