using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class PetService : IPetService
{
    private readonly IPetRepository _repository;

    public PetService(IPetRepository repository)
    {
        _repository = repository;
    }

    public List<PetResponse> GetAll()
    {
        var pets = _repository.GetAll();
        return pets.Select(pet => new PetResponse(pet)).ToList();
    }

    public PetResponse? GetById(int id)
    {
        var pet = _repository.GetById(id);
        return pet is null ? null : new PetResponse(pet);
    }

    public PetResponse Create(PetRequest request)
    {
        var pet = request.ToDomain();
        _repository.Add(pet);
        _repository.SaveChanges();
        return new PetResponse(pet);
    }

    public PetResponse? Update(int id, PetRequest request)
    {
        var pet = _repository.GetById(id);
        if (pet is null)
            return null;

        pet.Update(request.Nome, request.Especie, request.Raca, request.Sexo, request.DataNascimento, request.FotoUrl);
        _repository.Update(pet);
        _repository.SaveChanges();
        return new PetResponse(pet);
    }

    public PetResponse? Patch(int id, PetPatchRequest request)
    {
        var pet = _repository.GetById(id);
        if (pet is null)
            return null;

        var nome = request.Nome ?? pet.Nome;
        var especie = request.Especie ?? pet.Especie;
        var raca = request.Raca ?? pet.Raca;
        var sexo = request.Sexo ?? pet.Sexo;
        var dataNascimento = request.DataNascimento ?? pet.DataNascimento;
        var fotoUrl = request.FotoUrl ?? pet.FotoUrl;

        pet.Update(nome, especie, raca, sexo, dataNascimento, fotoUrl);
        _repository.Update(pet);
        _repository.SaveChanges();
        return new PetResponse(pet);
    }

    public bool Delete(int id)
    {
        var pet = _repository.GetById(id);
        if (pet is null)
            return false;

        _repository.Delete(pet);
        _repository.SaveChanges();
        return true;
    }
}