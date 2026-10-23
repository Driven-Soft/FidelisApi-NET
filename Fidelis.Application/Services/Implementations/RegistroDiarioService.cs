using System.Diagnostics;
using System.Diagnostics.Metrics;
using Fidelis.Application.Common;
using Fidelis.Application.Diagnostics;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fidelis.Application.Services.Implementations;

public class RegistroDiarioService : IRegistroDiarioService
{
    private readonly IRegistroDiarioRepository _repository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<RegistroDiarioService> _logger;

    private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

    private static readonly Meter Meter = new(TelemetryConstants.MeterName);
    private static readonly Counter<int> RegistrosCreatedCounter =
        Meter.CreateCounter<int>("registros_diario_created_total", description: "Total de registros de diário criados.");

    public RegistroDiarioService(
        IRegistroDiarioRepository repository,
        IPetRepository petRepository,
        ILogger<RegistroDiarioService> logger)
    {
        _repository = repository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public PagedResult<RegistroDiarioResponse> GetAll(RegistroDiarioQueryParameters query)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.GetAll");

        var result = _repository.GetPaged(query);

        _logger.LogInformation(
            "Consulta de registros de diário retornou {RegistroCount} registros de {TotalItems}",
            result.Items.Count,
            result.TotalItems);

        return result.Map(registro => new RegistroDiarioResponse(registro));
    }

    public RegistroDiarioResponse? GetById(string id)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.GetById");
        activity?.SetTag("registro.id", id);

        var registro = _repository.GetById(id);

        if (registro is null)
        {
            _logger.LogWarning("Registro de diário {RegistroId} não encontrado", id);
            return null;
        }

        return new RegistroDiarioResponse(registro);
    }

    public RegistroDiarioResponse Create(RegistroDiarioRequest request)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.Create");
        activity?.SetTag("registro.petId", request.PetId);

        if (_petRepository.GetById(request.PetId) is null)
            throw new ArgumentException($"Pet {request.PetId} não encontrado.");

        var registro = request.ToDomain();
        _repository.Add(registro);

        _logger.LogInformation(
            "Registro de diário criado. RegistroId: {RegistroId}, PetId: {PetId}, Humor: {Humor}",
            registro.Id,
            registro.PetId,
            registro.Humor);

        RegistrosCreatedCounter.Add(1, new KeyValuePair<string, object?>("humor", registro.Humor));

        return new RegistroDiarioResponse(registro);
    }

    public RegistroDiarioResponse? Update(string id, RegistroDiarioUpdateRequest request)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.Update");
        activity?.SetTag("registro.id", id);

        var registro = _repository.GetById(id);
        if (registro is null)
        {
            _logger.LogWarning("Tentativa de atualizar registro de diário inexistente. RegistroId: {RegistroId}", id);
            return null;
        }

        registro.Update(request.Titulo, request.Descricao, request.Humor, request.Tags, request.DataRegistro);
        _repository.Replace(registro);

        _logger.LogInformation("Registro de diário {RegistroId} atualizado com sucesso", id);

        return new RegistroDiarioResponse(registro);
    }

    public RegistroDiarioResponse? Patch(string id, RegistroDiarioPatchRequest request)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.Patch");
        activity?.SetTag("registro.id", id);

        var registro = _repository.GetById(id);
        if (registro is null)
        {
            _logger.LogWarning("Tentativa de atualizar parcialmente registro de diário inexistente. RegistroId: {RegistroId}", id);
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Titulo))
            registro.UpdateTitulo(request.Titulo);

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            registro.UpdateDescricao(request.Descricao);

        if (!string.IsNullOrWhiteSpace(request.Humor))
            registro.UpdateHumor(request.Humor);

        if (request.Tags is not null)
            registro.UpdateTags(request.Tags);

        if (request.DataRegistro.HasValue)
            registro.UpdateDataRegistro(request.DataRegistro.Value);

        _repository.Replace(registro);

        _logger.LogInformation("Registro de diário {RegistroId} atualizado parcialmente com sucesso", id);

        return new RegistroDiarioResponse(registro);
    }

    public bool Delete(string id)
    {
        using var activity = ActivitySource.StartActivity("RegistroDiario.Delete");
        activity?.SetTag("registro.id", id);

        var removido = _repository.Delete(id);

        if (removido)
            _logger.LogInformation("Registro de diário {RegistroId} removido com sucesso", id);
        else
            _logger.LogWarning("Tentativa de remover registro de diário inexistente. RegistroId: {RegistroId}", id);

        return removido;
    }
}
