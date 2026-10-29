using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
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

public class HistoricoPesoCriadoResponse : HistoricoPesoResponse
{
    public AvaliacaoPeso? AvaliacaoIa { get; }

    public HistoricoPesoCriadoResponse(HistoricoPeso historico, AvaliacaoPeso? avaliacaoIa) : base(historico)
    {
        AvaliacaoIa = avaliacaoIa;
    }
}

public class HistoricoPesoQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public decimal? PesoMinimo { get; set; }

    public decimal? PesoMaximo { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public IReadOnlyList<Expression<Func<HistoricoPeso, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var pesoMinimo = PesoMinimo.GetValueOrDefault();
        var pesoMaximo = PesoMaximo.GetValueOrDefault();
        var dataInicio = DataInicio.GetValueOrDefault();
        var dataFim = DataFim.GetValueOrDefault();

        return new FilterBuilder<HistoricoPeso>()
            .AddIf(PetId.HasValue, historico => historico.PetId == petId)
            .AddIf(PesoMinimo.HasValue, historico => historico.PesoKg >= pesoMinimo)
            .AddIf(PesoMaximo.HasValue, historico => historico.PesoKg <= pesoMaximo)
            .AddIf(DataInicio.HasValue, historico => historico.DataMedicao >= dataInicio)
            .AddIf(DataFim.HasValue, historico => historico.DataMedicao <= dataFim)
            .Build();
    }
}
