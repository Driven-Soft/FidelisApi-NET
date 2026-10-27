using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.Services.Interfaces;

public interface IAnalisePesoService
{
    AvaliacaoPeso? AnalisarPesagem(HistoricoPeso pesagem);
    AvaliacaoPesoResponse? GetAvaliacao(int petId);
}
