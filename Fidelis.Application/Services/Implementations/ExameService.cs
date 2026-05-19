using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class ExameService : IExameService
{
    private readonly IExameRepository _repository;

    public ExameService(IExameRepository repository)
    {
        _repository = repository;
    }

    public List<ExameResponse> GetAll()
    {
        var exames = _repository.GetAll();
        return exames.Select(exame => new ExameResponse(exame)).ToList();
    }

    public ExameResponse? GetById(int id)
    {
        var exame = _repository.GetById(id);
        return exame is null ? null : new ExameResponse(exame);
    }

    public ExameResponse Create(ExameRequest request)
    {
        var exame = request.ToDomain();
        _repository.Add(exame);
        _repository.SaveChanges();
        return new ExameResponse(exame);
    }

    public ExameResponse? Update(int id, ExameRequest request)
    {
        var exame = _repository.GetById(id);
        if (exame is null)
            return null;

        exame.UpdateTipo(request.Tipo);
        exame.UpdateDescricao(request.Descricao);
        exame.UpdateResultado(request.Resultado);
        _repository.Update(exame);
        _repository.SaveChanges();
        return new ExameResponse(exame);
    }

    public ExameResponse? Patch(int id, ExamePatchRequest request)
    {
        var exame = _repository.GetById(id);
        if (exame is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Tipo))
            exame.UpdateTipo(request.Tipo);

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            exame.UpdateDescricao(request.Descricao);

        if (request.Resultado is not null)
            exame.UpdateResultado(request.Resultado);

        _repository.Update(exame);
        _repository.SaveChanges();
        return new ExameResponse(exame);
    }

    public bool Delete(int id)
    {
        var exame = _repository.GetById(id);
        if (exame is null)
            return false;

        _repository.Delete(exame);
        _repository.SaveChanges();
        return true;
    }
}
