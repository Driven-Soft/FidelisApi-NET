using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class MedicamentoService : IMedicamentoService
{
    private readonly IMedicamentoRepository _repository;

    public MedicamentoService(IMedicamentoRepository repository)
    {
        _repository = repository;
    }

    public List<MedicamentoResponse> GetAll()
    {
        var medicamentos = _repository.GetAll();
        return medicamentos.Select(medicamento => new MedicamentoResponse(medicamento)).ToList();
    }

    public MedicamentoResponse? GetById(int id)
    {
        var medicamento = _repository.GetById(id);
        return medicamento is null ? null : new MedicamentoResponse(medicamento);
    }

    public MedicamentoResponse Create(MedicamentoRequest request)
    {
        var medicamento = request.ToDomain();
        _repository.Add(medicamento);
        _repository.SaveChanges();
        return new MedicamentoResponse(medicamento);
    }

    public MedicamentoResponse? Update(int id, MedicamentoRequest request)
    {
        var medicamento = _repository.GetById(id);
        if (medicamento is null)
            return null;

        medicamento.Update(request.Nome, request.Descricao);
        _repository.Update(medicamento);
        _repository.SaveChanges();
        return new MedicamentoResponse(medicamento);
    }

    public MedicamentoResponse? Patch(int id, MedicamentoPatchRequest request)
    {
        var medicamento = _repository.GetById(id);
        if (medicamento is null)
            return null;

        if (!string.IsNullOrWhiteSpace(request.Nome))
            medicamento.UpdateNome(request.Nome);

        if (!string.IsNullOrWhiteSpace(request.Descricao))
            medicamento.UpdateDescricao(request.Descricao);

        _repository.Update(medicamento);
        _repository.SaveChanges();
        return new MedicamentoResponse(medicamento);
    }

    public bool Delete(int id)
    {
        var medicamento = _repository.GetById(id);
        if (medicamento is null)
            return false;

        _repository.Delete(medicamento);
        _repository.SaveChanges();
        return true;
    }
}
