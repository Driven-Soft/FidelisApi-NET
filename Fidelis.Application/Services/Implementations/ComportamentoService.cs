using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class ComportamentoService : IComportamentoService
{
    private readonly IComportamentoRepository _repository;

    public ComportamentoService(IComportamentoRepository repository)
    {
        _repository = repository;
    }

    public List<ComportamentoResponse> GetAll()
    {
        var comportamentos = _repository.GetAll();
        return comportamentos.Select(comportamento => new ComportamentoResponse(comportamento)).ToList();
    }

    public ComportamentoResponse? GetById(int id)
    {
        var comportamento = _repository.GetById(id);
        return comportamento is null ? null : new ComportamentoResponse(comportamento);
    }

    public ComportamentoResponse Create(ComportamentoRequest request)
    {
        var comportamento = request.ToDomain();
        _repository.Add(comportamento);
        _repository.SaveChanges();
        return new ComportamentoResponse(comportamento);
    }

    public ComportamentoResponse? Update(int id, ComportamentoRequest request)
    {
        var comportamento = _repository.GetById(id);
        if (comportamento is null)
            return null;

        comportamento.UpdateDescricao(request.Descricao);
        _repository.Update(comportamento);
        _repository.SaveChanges();
        return new ComportamentoResponse(comportamento);
    }

    public ComportamentoResponse? Patch(int id, ComportamentoPatchRequest request)
    {
        var comportamento = _repository.GetById(id);
        if (comportamento is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            comportamento.UpdateDescricao(request.Descricao);

        _repository.Update(comportamento);
        _repository.SaveChanges();
        return new ComportamentoResponse(comportamento);
    }

    public bool Delete(int id)
    {
        var comportamento = _repository.GetById(id);
        if (comportamento is null)
            return false;

        _repository.Delete(comportamento);
        _repository.SaveChanges();
        return true;
    }
}
