using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class ComportamentoRequest
{
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime Data { get; set; }

    [Required]
    public int PetId { get; set; }

    public Comportamento ToDomain() => new(Descricao, Data, PetId);
}

public class ComportamentoPatchRequest
{
    public string? Descricao { get; set; }
}

public class ComportamentoResponse
{
    public int Id { get; }
    public DateTime Data { get; }
    public string Descricao { get; }
    public int PetId { get; }

    public ComportamentoResponse(Comportamento comportamento)
    {
        Id = comportamento.Id;
        Data = comportamento.Data;
        Descricao = comportamento.Descricao;
        PetId = comportamento.PetId;
    }
}

public class ComportamentoQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public string? Descricao { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public IReadOnlyList<Expression<Func<Comportamento, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var descricao = TextFilter.Normalize(Descricao);
        var dataInicio = DataInicio.GetValueOrDefault();
        var dataFim = DataFim.GetValueOrDefault();

        return new FilterBuilder<Comportamento>()
            .AddIf(PetId.HasValue, comportamento => comportamento.PetId == petId)
            .AddIf(descricao is not null, comportamento => comportamento.Descricao.ToUpper().Contains(descricao!))
            .AddIf(DataInicio.HasValue, comportamento => comportamento.Data >= dataInicio)
            .AddIf(DataFim.HasValue, comportamento => comportamento.Data <= dataFim)
            .Build();
    }
}
