using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class VacinacaoService : IVacinacaoService
{
    private readonly IVacinacaoRepository _repository;

    public VacinacaoService(IVacinacaoRepository repository)
    {
        _repository = repository;
    }

    public List<VacinacaoResponse> GetAll()
    {
        var vacinacoes = _repository.GetAll();
        return vacinacoes.Select(vacinacao => new VacinacaoResponse(vacinacao)).ToList();
    }

    public VacinacaoResponse? GetById(int id)
    {
        var vacinacao = _repository.GetById(id);
        return vacinacao is null ? null : new VacinacaoResponse(vacinacao);
    }

    public VacinacaoResponse Create(VacinacaoRequest request)
    {
        var vacinacao = request.ToDomain();
        _repository.Add(vacinacao);
        _repository.SaveChanges();
        return new VacinacaoResponse(vacinacao);
    }

    public VacinacaoResponse? Update(int id, VacinacaoRequest request)
    {
        var vacinacao = _repository.GetById(id);
        if (vacinacao is null)
            return null;

        vacinacao.UpdateVacinaAplicada(request.VacinaAplicada);
        vacinacao.UpdateDatas(request.DataAplicacao, request.DataProxima);
        _repository.Update(vacinacao);
        _repository.SaveChanges();
        return new VacinacaoResponse(vacinacao);
    }

    public VacinacaoResponse? Patch(int id, VacinacaoPatchRequest request)
    {
        var vacinacao = _repository.GetById(id);
        if (vacinacao is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.VacinaAplicada))
            vacinacao.UpdateVacinaAplicada(request.VacinaAplicada);

        if (request.DataAplicacao.HasValue && request.DataProxima.HasValue)
            vacinacao.UpdateDatas(request.DataAplicacao.Value, request.DataProxima.Value);

        _repository.Update(vacinacao);
        _repository.SaveChanges();
        return new VacinacaoResponse(vacinacao);
    }

    public bool Delete(int id)
    {
        var vacinacao = _repository.GetById(id);
        if (vacinacao is null)
            return false;

        _repository.Delete(vacinacao);
        _repository.SaveChanges();
        return true;
    }
}
