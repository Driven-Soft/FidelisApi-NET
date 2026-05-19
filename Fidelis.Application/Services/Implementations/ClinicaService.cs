using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class ClinicaService : IClinicaService
{
    private readonly IClinicaRepository _repository;

    public ClinicaService(IClinicaRepository repository)
    {
        _repository = repository;
    }

    public List<ClinicaResponse> GetAll()
    {
        var clinicas = _repository.GetAll();
        return clinicas.Select(clinica => new ClinicaResponse(clinica)).ToList();
    }

    public ClinicaResponse? GetById(int id)
    {
        var clinica = _repository.GetById(id);
        return clinica is null ? null : new ClinicaResponse(clinica);
    }

    public ClinicaResponse Create(ClinicaRequest request)
    {
        var clinica = request.ToDomain();
        _repository.Add(clinica);
        _repository.SaveChanges();
        return new ClinicaResponse(clinica);
    }

    public ClinicaResponse? Update(int id, ClinicaRequest request)
    {
        var clinica = _repository.GetById(id);
        if (clinica is null)
            return null;

        clinica.Update(request.Nome, request.Cnpj, request.Telefone, request.Email, request.Endereco);
        _repository.Update(clinica);
        _repository.SaveChanges();
        return new ClinicaResponse(clinica);
    }

    public ClinicaResponse? Patch(int id, ClinicaPatchRequest request)
    {
        var clinica = _repository.GetById(id);
        if (clinica is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Nome))
            clinica.UpdateNome(request.Nome);

        if (!string.IsNullOrWhiteSpace(request.Cnpj))
            clinica.UpdateCnpj(request.Cnpj);

        if (!string.IsNullOrWhiteSpace(request.Telefone))
            clinica.UpdateTelefone(request.Telefone);

        if (!string.IsNullOrWhiteSpace(request.Email))
            clinica.UpdateEmail(request.Email);

        if (!string.IsNullOrWhiteSpace(request.Endereco))
            clinica.UpdateEndereco(request.Endereco);

        _repository.Update(clinica);
        _repository.SaveChanges();
        return new ClinicaResponse(clinica);
    }

    public bool Delete(int id)
    {
        var clinica = _repository.GetById(id);
        if (clinica is null)
            return false;

        _repository.Delete(clinica);
        _repository.SaveChanges();
        return true;
    }
}
