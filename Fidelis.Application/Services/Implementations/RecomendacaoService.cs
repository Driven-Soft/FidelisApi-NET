using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class RecomendacaoService : IRecomendacaoService
{
    private readonly IRecomendacaoRepository _repository;

    public RecomendacaoService(IRecomendacaoRepository repository)
    {
        _repository = repository;
    }

    public List<RecomendacaoResponse> GetAll()
    {
        var recomendacoes = _repository.GetAll();
        return recomendacoes.Select(recomendacao => new RecomendacaoResponse(recomendacao)).ToList();
    }

    public RecomendacaoResponse? GetById(int id)
    {
        var recomendacao = _repository.GetById(id);
        return recomendacao is null ? null : new RecomendacaoResponse(recomendacao);
    }

    public RecomendacaoResponse Create(RecomendacaoRequest request)
    {
        var recomendacao = request.ToDomain();
        _repository.Add(recomendacao);
        _repository.SaveChanges();
        return new RecomendacaoResponse(recomendacao);
    }

    public RecomendacaoResponse? Update(int id, RecomendacaoRequest request)
    {
        var recomendacao = _repository.GetById(id);
        if (recomendacao is null)
            return null;

        recomendacao.UpdateTipo(request.Tipo);
        recomendacao.UpdateDescricao(request.Descricao);
        _repository.Update(recomendacao);
        _repository.SaveChanges();
        return new RecomendacaoResponse(recomendacao);
    }

    public RecomendacaoResponse? Patch(int id, RecomendacaoPatchRequest request)
    {
        var recomendacao = _repository.GetById(id);
        if (recomendacao is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Tipo))
            recomendacao.UpdateTipo(request.Tipo);

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            recomendacao.UpdateDescricao(request.Descricao);

        _repository.Update(recomendacao);
        _repository.SaveChanges();
        return new RecomendacaoResponse(recomendacao);
    }

    public bool Delete(int id)
    {
        var recomendacao = _repository.GetById(id);
        if (recomendacao is null)
            return false;

        _repository.Delete(recomendacao);
        _repository.SaveChanges();
        return true;
    }
}
