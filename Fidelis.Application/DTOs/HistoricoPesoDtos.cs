using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class HistoricoPesoRequest
{
    [Required]
    public decimal PesoKg { get; set; }

    [Required]
    public DateTime DataMedicao { get; set; }

    public string? Observacao { get; set; }

    [Required]
    public int PetId { get; set; }

    public HistoricoPeso ToDomain() => new(PesoKg, DataMedicao, PetId, Observacao);
}

public class HistoricoPesoPatchRequest
{
    public decimal? PesoKg { get; set; }
}

public class HistoricoPesoResponse
{
    public int Id { get; }
    public decimal PesoKg { get; }
    public DateTime DataMedicao { get; }
    public string? Observacao { get; }
    public int PetId { get; }

    public HistoricoPesoResponse(HistoricoPeso historico)
    {
        Id = historico.Id;
        PesoKg = historico.PesoKg;
        DataMedicao = historico.DataMedicao;
        Observacao = historico.Observacao;
        PetId = historico.PetId;
    }
}

