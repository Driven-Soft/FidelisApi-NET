using System.Diagnostics;
using System.Diagnostics.Metrics;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Diagnostics;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fidelis.Application.Services.Implementations;

public class PetService : IPetService
{
    private readonly IPetRepository _repository;
    private readonly ILogger<PetService> _logger;

    private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

    private static readonly Meter Meter = new(TelemetryConstants.MeterName);
    private static readonly Counter<int> PetsCreatedCounter =
        Meter.CreateCounter<int>("pets_created_total", description: "Total de pets cadastrados.");

    public PetService(IPetRepository repository, ILogger<PetService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public PagedResult<PetResponse> GetAll(PetQueryParameters query)
    {
        using var activity = ActivitySource.StartActivity("Pet.GetAll");
        activity?.SetTag("query.page", query.Page);
        activity?.SetTag("query.pageSize", query.PageSize);

        var result = _repository.GetPaged(query, query.ToFilters());

        _logger.LogInformation("Consulta de pets retornou {PetCount} registros de {TotalItems}", result.Items.Count, result.TotalItems);

        return result.Map(pet => new PetResponse(pet));
    }

    public PetResponse? GetById(int id)
    {
        using var activity = ActivitySource.StartActivity("Pet.GetById");
        activity?.SetTag("pet.id", id);

        var pet = _repository.GetById(id);

        if (pet is null)
        {
            _logger.LogWarning("Pet {PetId} não encontrado", id);
            return null;
        }

        return new PetResponse(pet);
    }

    public PetResponse Create(PetRequest request)
    {
        using var activity = ActivitySource.StartActivity("Pet.Create");
        activity?.SetTag("pet.nome", request.Nome);
        activity?.SetTag("pet.especie", request.Especie);

        var pet = request.ToDomain();
        _repository.Add(pet);
        _repository.SaveChanges();

        _logger.LogInformation("Pet cadastrado com sucesso: {@Pet}", new
        {
            pet.Id,
            pet.Nome,
            pet.Especie,
            pet.TutorId
        });

        PetsCreatedCounter.Add(1, new KeyValuePair<string, object?>("especie", pet.Especie));

        return new PetResponse(pet);
    }

    public PetResponse? Update(int id, PetRequest request)
    {
        using var activity = ActivitySource.StartActivity("Pet.Update");
        activity?.SetTag("pet.id", id);

        var pet = _repository.GetById(id);
        if (pet is null)
        {
            _logger.LogWarning("Tentativa de atualizar pet inexistente. PetId: {PetId}", id);
            return null;
        }

        pet.Update(request.Nome, request.Especie, request.Raca, request.Sexo, request.DataNascimento, request.FotoUrl);
        _repository.Update(pet);
        _repository.SaveChanges();

        _logger.LogInformation("Pet {PetId} atualizado com sucesso", id);

        return new PetResponse(pet);
    }

    public PetResponse? Patch(int id, PetPatchRequest request)
    {
        using var activity = ActivitySource.StartActivity("Pet.Patch");
        activity?.SetTag("pet.id", id);

        var pet = _repository.GetById(id);
        if (pet is null)
        {
            _logger.LogWarning("Tentativa de atualizar parcialmente pet inexistente. PetId: {PetId}", id);
            return null;
        }

        var nome = request.Nome ?? pet.Nome;
        var especie = request.Especie ?? pet.Especie;
        var raca = request.Raca ?? pet.Raca;
        var sexo = request.Sexo ?? pet.Sexo;
        var dataNascimento = request.DataNascimento ?? pet.DataNascimento;
        var fotoUrl = request.FotoUrl ?? pet.FotoUrl;

        pet.Update(nome, especie, raca, sexo, dataNascimento, fotoUrl);
        _repository.Update(pet);
        _repository.SaveChanges();

        _logger.LogInformation("Pet {PetId} atualizado parcialmente com sucesso", id);

        return new PetResponse(pet);
    }

    public bool Delete(int id)
    {
        using var activity = ActivitySource.StartActivity("Pet.Delete");
        activity?.SetTag("pet.id", id);

        var pet = _repository.GetById(id);
        if (pet is null)
        {
            _logger.LogWarning("Tentativa de remover pet inexistente. PetId: {PetId}", id);
            return false;
        }

        _repository.Delete(pet);
        _repository.SaveChanges();

        _logger.LogInformation("Pet {PetId} removido com sucesso", id);

        return true;
    }
}
