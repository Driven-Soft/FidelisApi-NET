using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class VermifugacaoService : IVermifugacaoService
{
    private readonly IVermifugacaoRepository _repository;

    public VermifugacaoService(IVermifugacaoRepository repository)
    {
        _repository = repository;
    }

    public List<VermifugacaoResponse> GetAll()
    {
        var vermifugacoes = _repository.GetAll();
        return vermifugacoes.Select(vermifugacao => new VermifugacaoResponse(vermifugacao)).ToList();
    }

    public VermifugacaoResponse? GetById(int id)
    {
        var vermifugacao = _repository.GetById(id);
        return vermifugacao is null ? null : new VermifugacaoResponse(vermifugacao);
    }

    public VermifugacaoResponse Create(VermifugacaoRequest request)
    {
        var vermifugacao = request.ToDomain();
        _repository.Add(vermifugacao);
        _repository.SaveChanges();
        return new VermifugacaoResponse(vermifugacao);
    }

    public VermifugacaoResponse? Update(int id, VermifugacaoRequest request)
    {
        var vermifugacao = _repository.GetById(id);
        if (vermifugacao is null)
            return null;

        vermifugacao.UpdateProduto(request.Produto);
        vermifugacao.UpdateDatas(request.DataAplicacao, request.DataProxima);
        _repository.Update(vermifugacao);
        _repository.SaveChanges();
        return new VermifugacaoResponse(vermifugacao);
    }

    public VermifugacaoResponse? Patch(int id, VermifugacaoPatchRequest request)
    {
        var vermifugacao = _repository.GetById(id);
        if (vermifugacao is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Produto))
            vermifugacao.UpdateProduto(request.Produto);

        if (request.DataAplicacao.HasValue && request.DataProxima.HasValue)
            vermifugacao.UpdateDatas(request.DataAplicacao.Value, request.DataProxima.Value);

        _repository.Update(vermifugacao);
        _repository.SaveChanges();
        return new VermifugacaoResponse(vermifugacao);
    }

    public bool Delete(int id)
    {
        var vermifugacao = _repository.GetById(id);
        if (vermifugacao is null)
            return false;

        _repository.Delete(vermifugacao);
        _repository.SaveChanges();
        return true;
    }
}
