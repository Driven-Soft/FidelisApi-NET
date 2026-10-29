using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class HistoricoPesoService : IHistoricoPesoService
{
    private readonly IHistoricoPesoRepository _repository;
    private readonly IAnalisePesoService _analisePesoService;

    public HistoricoPesoService(IHistoricoPesoRepository repository, IAnalisePesoService analisePesoService)
    {
        _repository = repository;
        _analisePesoService = analisePesoService;
    }

    public PagedResult<HistoricoPesoResponse> GetAll(HistoricoPesoQueryParameters query)
    {
        return _repository
            .GetPaged(query, query.ToFilters())
            .Map(historicoPeso => new HistoricoPesoResponse(historicoPeso));
    }

    public HistoricoPesoResponse? GetById(int id)
    {
        var historico = _repository.GetById(id);
        return historico is null ? null : new HistoricoPesoResponse(historico);
    }

    public HistoricoPesoCriadoResponse Create(HistoricoPesoRequest request)
    {
        var historico = request.ToDomain();
        _repository.Add(historico);
        _repository.SaveChanges();
        var avaliacao = _analisePesoService.AnalisarPesagem(historico);
        return new HistoricoPesoCriadoResponse(historico, avaliacao);
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

    public AvaliacaoPesoResponse? GetAvaliacao(int petId)
    {
        return _analisePesoService.GetAvaliacao(petId);
    }
}
