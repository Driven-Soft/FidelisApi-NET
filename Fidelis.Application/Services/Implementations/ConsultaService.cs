using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _repository;

    public ConsultaService(IConsultaRepository repository)
    {
        _repository = repository;
    }

    public List<ConsultaResponse> GetAll()
    {
        var consultas = _repository.GetAll();
        return consultas.Select(consulta => new ConsultaResponse(consulta)).ToList();
    }

    public ConsultaResponse? GetById(int id)
    {
        var consulta = _repository.GetById(id);
        return consulta is null ? null : new ConsultaResponse(consulta);
    }

    public ConsultaResponse Create(ConsultaRequest request)
    {
        var consulta = request.ToDomain();
        _repository.Add(consulta);
        _repository.SaveChanges();
        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse? Update(int id, ConsultaRequest request)
    {
        var consulta = _repository.GetById(id);
        if (consulta is null)
            return null;

        consulta.Update(request.Tipo, request.Diagnostico, request.Observacoes, request.DataRetorno);
        _repository.Update(consulta);
        _repository.SaveChanges();
        return new ConsultaResponse(consulta);
    }

    public ConsultaResponse? Patch(int id, ConsultaPatchRequest request)
    {
        var consulta = _repository.GetById(id);
        if (consulta is null)
            return null;

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
        return new ConsultaResponse(consulta);
    }

    public bool Delete(int id)
    {
        var consulta = _repository.GetById(id);
        if (consulta is null)
            return false;

        _repository.Delete(consulta);
        _repository.SaveChanges();
        return true;
    }
}
