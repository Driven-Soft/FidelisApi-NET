using System.Diagnostics;
using System.Diagnostics.Metrics;
using Fidelis.Application.DTOs;
using Fidelis.Application.Diagnostics;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fidelis.Application.Services.Implementations;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _repository;
    private readonly ILogger<ConsultaService> _logger;

    private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

    private static readonly Meter Meter = new(TelemetryConstants.MeterName);
    private static readonly Counter<int> ConsultasCreatedCounter =
        Meter.CreateCounter<int>("consultas_created_total", description: "Total de consultas registradas.");

    public ConsultaService(IConsultaRepository repository, ILogger<ConsultaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<ConsultaResponse> GetAll()
    {
        using var activity = ActivitySource.StartActivity("Consulta.GetAll");

        var consultas = _repository.GetAll();

        _logger.LogInformation("Consulta de consultas retornou {ConsultaCount} registros", consultas.Count);

        return consultas.Select(consulta => new ConsultaResponse(consulta)).ToList();
    }

    public ConsultaResponse? GetById(int id)
    {
        using var activity = ActivitySource.StartActivity("Consulta.GetById");
        activity?.SetTag("consulta.id", id);

        var consulta = _repository.GetById(id);

        if (consulta is null)
        {
            _logger.LogWarning("Consulta {ConsultaId} não encontrada", id);
            return null;
        }

        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse Create(ConsultaRequest request)
    {
        using var activity = ActivitySource.StartActivity("Consulta.Create");
        activity?.SetTag("consulta.tipo", request.Tipo);
        activity?.SetTag("consulta.petId", request.PetId);

        var consulta = request.ToDomain();
        _repository.Add(consulta);
        _repository.SaveChanges();

        _logger.LogInformation(
            "Consulta registrada com sucesso. ConsultaId: {ConsultaId}, PetId: {PetId}, Tipo: {Tipo}",
            consulta.Id,
            consulta.PetId,
            consulta.Tipo);

        ConsultasCreatedCounter.Add(1, new KeyValuePair<string, object?>("tipo", consulta.Tipo));

        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse? Update(int id, ConsultaRequest request)
    {
        using var activity = ActivitySource.StartActivity("Consulta.Update");
        activity?.SetTag("consulta.id", id);

        var consulta = _repository.GetById(id);
        if (consulta is null)
        {
            _logger.LogWarning("Tentativa de atualizar consulta inexistente. ConsultaId: {ConsultaId}", id);
            return null;
        }

        consulta.Update(request.Tipo, request.Diagnostico, request.Observacoes, request.DataRetorno);
        _repository.Update(consulta);
        _repository.SaveChanges();

        _logger.LogInformation("Consulta {ConsultaId} atualizada com sucesso", id);

        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse? Patch(int id, ConsultaPatchRequest request)
    {
        using var activity = ActivitySource.StartActivity("Consulta.Patch");
        activity?.SetTag("consulta.id", id);

        var consulta = _repository.GetById(id);
        if (consulta is null)
        {
            _logger.LogWarning("Tentativa de atualizar parcialmente consulta inexistente. ConsultaId: {ConsultaId}", id);
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
            consulta.UpdateTipo(request.Tipo);

        if (request.Diagnostico is not null)
            consulta.UpdateDiagnostico(request.Diagnostico);

        if (request.Observacoes is not null)
            consulta.UpdateObservacoes(request.Observacoes);

        if (request.DataRetorno.HasValue)
            consulta.UpdateDataRetorno(request.DataRetorno);

        _repository.Update(consulta);
        _repository.SaveChanges();

        _logger.LogInformation("Consulta {ConsultaId} atualizada parcialmente com sucesso", id);

        return new ConsultaResponse(consulta);
    }

    public bool Delete(int id)
    {
        using var activity = ActivitySource.StartActivity("Consulta.Delete");
        activity?.SetTag("consulta.id", id);

        var consulta = _repository.GetById(id);
        if (consulta is null)
        {
            _logger.LogWarning("Tentativa de remover consulta inexistente. ConsultaId: {ConsultaId}", id);
            return false;
        }

        _repository.Delete(consulta);
        _repository.SaveChanges();

        _logger.LogInformation("Consulta {ConsultaId} removida com sucesso", id);

        return true;
    }
}
