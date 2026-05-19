using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class LembreteService : ILembreteService
{
    private readonly ILembreteRepository _repository;

    public LembreteService(ILembreteRepository repository)
    {
        _repository = repository;
    }

    public List<LembreteResponse> GetAll()
    {
        var lembretes = _repository.GetAll();
        return lembretes.Select(lembrete => new LembreteResponse(lembrete)).ToList();
    }

    public LembreteResponse? GetById(int id)
    {
        var lembrete = _repository.GetById(id);
        return lembrete is null ? null : new LembreteResponse(lembrete);
    }

    public LembreteResponse Create(LembreteRequest request)
    {
        var lembrete = request.ToDomain();
        _repository.Add(lembrete);
        _repository.SaveChanges();
        return new LembreteResponse(lembrete);
    }

    public LembreteResponse? Update(int id, LembreteRequest request)
    {
        var lembrete = _repository.GetById(id);
        if (lembrete is null)
            return null;

        lembrete.UpdateTipo(request.Tipo);
        lembrete.UpdateDescricao(request.Descricao);
        _repository.Update(lembrete);
        _repository.SaveChanges();
        return new LembreteResponse(lembrete);
    }

    public LembreteResponse? Patch(int id, LembretePatchRequest request)
    {
        var lembrete = _repository.GetById(id);
        if (lembrete is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Tipo))
            lembrete.UpdateTipo(request.Tipo);

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            lembrete.UpdateDescricao(request.Descricao);

        if (request.Status == 'C')
            lembrete.Concluir();
        else if (request.Status == 'X')
            lembrete.Cancelar();

        _repository.Update(lembrete);
        _repository.SaveChanges();
        return new LembreteResponse(lembrete);
    }

    public bool Delete(int id)
    {
        var lembrete = _repository.GetById(id);
        if (lembrete is null)
            return false;

        _repository.Delete(lembrete);
        _repository.SaveChanges();
        return true;
    }
}
