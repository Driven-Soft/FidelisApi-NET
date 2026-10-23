using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class RecomendacaoRequest
{
    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime DataRecomendacao { get; set; }

    [Required]
    public int PetId { get; set; }

    public Recomendacao ToDomain() => new(Tipo, Descricao, DataRecomendacao, PetId);
}

public class RecomendacaoPatchRequest
{
    [MaxLength(50)]
    public string? Tipo { get; set; }

    public string? Descricao { get; set; }
}

public class RecomendacaoResponse
{
    public int Id { get; }
    public string Tipo { get; }
    public string Descricao { get; }
    public DateTime DataRecomendacao { get; }
    public int PetId { get; }

    public RecomendacaoResponse(Recomendacao recomendacao)
    {
        Id = recomendacao.Id;
        Tipo = recomendacao.Tipo;
        Descricao = recomendacao.Descricao;
        DataRecomendacao = recomendacao.DataRecomendacao;
        PetId = recomendacao.PetId;
    }
}

public class RecomendacaoQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public string? Tipo { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public IReadOnlyList<Expression<Func<Recomendacao, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var tipo = TextFilter.Normalize(Tipo);
        var dataInicio = DataInicio.GetValueOrDefault();
        var dataFim = DataFim.GetValueOrDefault();

        return new FilterBuilder<Recomendacao>()
            .AddIf(PetId.HasValue, recomendacao => recomendacao.PetId == petId)
            .AddIf(tipo is not null, recomendacao => recomendacao.Tipo.ToUpper().Contains(tipo!))
            .AddIf(DataInicio.HasValue, recomendacao => recomendacao.DataRecomendacao >= dataInicio)
            .AddIf(DataFim.HasValue, recomendacao => recomendacao.DataRecomendacao <= dataFim)
            .Build();
    }
}
