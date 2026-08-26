using System.Diagnostics;
using System.Diagnostics.Metrics;
using Fidelis.Application.DTOs;
using Fidelis.Application.Diagnostics;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Fidelis.Application.Services.Implementations;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _repository;
    private readonly ILogger<TutorService> _logger;

    private static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

    private static readonly Meter Meter = new(TelemetryConstants.MeterName);
    private static readonly Counter<int> TutoresCreatedCounter =
        Meter.CreateCounter<int>("tutores_created_total", description: "Total de tutores cadastrados.");

    public TutorService(ITutorRepository repository, ILogger<TutorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<TutorResponse> GetAll()
    {
        using var activity = ActivitySource.StartActivity("Tutor.GetAll");

        var tutores = _repository.GetAll();

        _logger.LogInformation("Consulta de tutores retornou {TutorCount} registros", tutores.Count);

        return tutores.Select(tutor => new TutorResponse(tutor)).ToList();
    }

    public TutorResponse? GetById(int id)
    {
        using var activity = ActivitySource.StartActivity("Tutor.GetById");
        activity?.SetTag("tutor.id", id);

        var tutor = _repository.GetById(id);

        if (tutor is null)
        {
            _logger.LogWarning("Tutor {TutorId} não encontrado", id);
            return null;
        }

        return new TutorResponse(tutor);
    }

    public TutorResponse Create(TutorRequest request)
    {
        using var activity = ActivitySource.StartActivity("Tutor.Create");
        activity?.SetTag("tutor.nome", request.Nome);

        var tutor = request.ToDomain();
        _repository.Add(tutor);
        _repository.SaveChanges();

        _logger.LogInformation(
            "Tutor cadastrado com sucesso. TutorId: {TutorId}, Nome: {TutorNome}",
            tutor.Id,
            tutor.Nome);

        TutoresCreatedCounter.Add(1);

        return new TutorResponse(tutor);
    }

    public TutorResponse? Update(int id, TutorRequest request)
    {
        using var activity = ActivitySource.StartActivity("Tutor.Update");
        activity?.SetTag("tutor.id", id);

        var tutor = _repository.GetById(id);
        if (tutor is null)
        {
            _logger.LogWarning("Tentativa de atualizar tutor inexistente. TutorId: {TutorId}", id);
            return null;
        }

        tutor.Update(request.Cpf, request.Nome, request.Email, request.Telefone, request.Endereco);
        _repository.Update(tutor);
        _repository.SaveChanges();

        _logger.LogInformation("Tutor {TutorId} atualizado com sucesso", id);

        return new TutorResponse(tutor);
    }

    public TutorResponse? Patch(int id, TutorPatchRequest request)
    {
        using var activity = ActivitySource.StartActivity("Tutor.Patch");
        activity?.SetTag("tutor.id", id);

        var tutor = _repository.GetById(id);
        if (tutor is null)
        {
            _logger.LogWarning("Tentativa de atualizar parcialmente tutor inexistente. TutorId: {TutorId}", id);
            return null;
        }

        var cpf = request.Cpf ?? tutor.Cpf;
        var nome = request.Nome ?? tutor.Nome;
        var email = request.Email ?? tutor.Email;
        var telefone = request.Telefone ?? tutor.Telefone;
        var endereco = request.Endereco ?? tutor.Endereco;

        tutor.Update(cpf, nome, email, telefone, endereco);
        _repository.Update(tutor);
        _repository.SaveChanges();

        _logger.LogInformation("Tutor {TutorId} atualizado parcialmente com sucesso", id);

        return new TutorResponse(tutor);
    }

    public bool Delete(int id)
    {
        using var activity = ActivitySource.StartActivity("Tutor.Delete");
        activity?.SetTag("tutor.id", id);

        var tutor = _repository.GetById(id);
        if (tutor is null)
        {
            _logger.LogWarning("Tentativa de remover tutor inexistente. TutorId: {TutorId}", id);
            return false;
        }

        _repository.Delete(tutor);
        _repository.SaveChanges();

        _logger.LogInformation("Tutor {TutorId} removido com sucesso", id);

        return true;
    }
}
