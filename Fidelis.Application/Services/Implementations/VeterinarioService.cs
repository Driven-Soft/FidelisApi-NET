using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class VeterinarioService : IVeterinarioService
{
    private readonly IVeterinarioRepository _repository;

    public VeterinarioService(IVeterinarioRepository repository)
    {
        _repository = repository;
    }

    public List<VeterinarioResponse> GetAll()
    {
        var veterinarios = _repository.GetAll();
        return veterinarios.Select(veterinario => new VeterinarioResponse(veterinario)).ToList();
    }

    public VeterinarioResponse? GetById(int id)
    {
        var veterinario = _repository.GetById(id);
        return veterinario is null ? null : new VeterinarioResponse(veterinario);
    }

    public VeterinarioResponse Create(VeterinarioRequest request)
    {
        var veterinario = request.ToDomain();
        _repository.Add(veterinario);
        _repository.SaveChanges();
        return new VeterinarioResponse(veterinario);
    }

    public VeterinarioResponse? Update(int id, VeterinarioRequest request)
    {
        var veterinario = _repository.GetById(id);
        if (veterinario is null)
            return null;

        veterinario.Update(request.Crmv, request.Nome, request.Email, request.Especialidade);
        _repository.Update(veterinario);
        _repository.SaveChanges();
        return new VeterinarioResponse(veterinario);
    }

    public VeterinarioResponse? Patch(int id, VeterinarioPatchRequest request)
    {
        var veterinario = _repository.GetById(id);
        if (veterinario is null)
            return null;

        var crmv = request.Crmv ?? veterinario.Crmv;
        var nome = request.Nome ?? veterinario.Nome;
        var email = request.Email ?? veterinario.Email;
        var especialidade = request.Especialidade ?? veterinario.Especialidade;

        veterinario.Update(crmv, nome, email, especialidade);
        _repository.Update(veterinario);
        _repository.SaveChanges();
        return new VeterinarioResponse(veterinario);
    }

    public bool Delete(int id)
    {
        var veterinario = _repository.GetById(id);
        if (veterinario is null)
            return false;

        _repository.Delete(veterinario);
        _repository.SaveChanges();
        return true;
    }
}