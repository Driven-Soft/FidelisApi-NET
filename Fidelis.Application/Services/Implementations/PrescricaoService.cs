using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Application.Services.Implementations;

public class PrescricaoService : IPrescricaoService
{
    private readonly IPrescricaoRepository _repository;

    public PrescricaoService(IPrescricaoRepository repository)
    {
        _repository = repository;
    }

    public List<PrescricaoResponse> GetAll()
    {
        var prescricoes = _repository.GetAll();
        return prescricoes.Select(prescricao => new PrescricaoResponse(prescricao)).ToList();
    }

    public PrescricaoResponse? GetById(int id)
    {
        var prescricao = _repository.GetById(id);
        return prescricao is null ? null : new PrescricaoResponse(prescricao);
    }

    public PrescricaoResponse Create(PrescricaoRequest request)
    {
        var prescricao = request.ToDomain();
        _repository.Add(prescricao);
        _repository.SaveChanges();
        return new PrescricaoResponse(prescricao);
    }

    public PrescricaoResponse? Update(int id, PrescricaoRequest request)
    {
        var prescricao = _repository.GetById(id);
        if (prescricao is null)
            return null;

        prescricao.Update(request.Dosagem, request.Frequencia, request.DuracaoDias, request.Observacao);
        _repository.Update(prescricao);
        _repository.SaveChanges();
        return new PrescricaoResponse(prescricao);
    }

    public PrescricaoResponse? Patch(int id, PrescricaoPatchRequest request)
    {
        var prescricao = _repository.GetById(id);
        if (prescricao is null)
            return null;

        var dosagem = request.Dosagem ?? prescricao.Dosagem;
        var frequencia = request.Frequencia ?? prescricao.Frequencia;
        var duracaoDias = request.DuracaoDias ?? prescricao.DuracaoDias;
        var observacao = request.Observacao ?? prescricao.Observacao;

        prescricao.Update(dosagem, frequencia, duracaoDias, observacao);
        _repository.Update(prescricao);
        _repository.SaveChanges();
        return new PrescricaoResponse(prescricao);
    }

    public bool Delete(int id)
    {
        var prescricao = _repository.GetById(id);
        if (prescricao is null)
            return false;

        _repository.Delete(prescricao);
        _repository.SaveChanges();
        return true;
    }
}
