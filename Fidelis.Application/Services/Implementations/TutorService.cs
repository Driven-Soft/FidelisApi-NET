using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _repository;

    public TutorService(ITutorRepository repository)
    {
        _repository = repository;
    }

    public List<TutorResponse> GetAll()
    {
        var tutores = _repository.GetAll();
        return tutores.Select(tutor => new TutorResponse(tutor)).ToList();
    }

    public TutorResponse? GetById(int id)
    {
        var tutor = _repository.GetById(id);
        return tutor is null ? null : new TutorResponse(tutor);
    }

    public TutorResponse Create(TutorRequest request)
    {
        var tutor = request.ToDomain();
        _repository.Add(tutor);
        _repository.SaveChanges();
        return new TutorResponse(tutor);
    }

    public TutorResponse? Update(int id, TutorRequest request)
    {
        var tutor = _repository.GetById(id);
        if (tutor is null)
            return null;

        tutor.Update(request.Cpf, request.Nome, request.Email, request.Telefone, request.Endereco);
        _repository.Update(tutor);
        _repository.SaveChanges();
        return new TutorResponse(tutor);
    }

    public TutorResponse? Patch(int id, TutorPatchRequest request)
    {
        var tutor = _repository.GetById(id);
        if (tutor is null)
            return null;

        var cpf = request.Cpf ?? tutor.Cpf;
        var nome = request.Nome ?? tutor.Nome;
        var email = request.Email ?? tutor.Email;
        var telefone = request.Telefone ?? tutor.Telefone;
        var endereco = request.Endereco ?? tutor.Endereco;

        tutor.Update(cpf, nome, email, telefone, endereco);
        _repository.Update(tutor);
        _repository.SaveChanges();
        return new TutorResponse(tutor);
    }

    public bool Delete(int id)
    {
        var tutor = _repository.GetById(id);
        if (tutor is null)
            return false;

        _repository.Delete(tutor);
        _repository.SaveChanges();
        return true;
    }
}