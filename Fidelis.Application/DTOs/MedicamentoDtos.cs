using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class MedicamentoRequest
{
    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public int PrescricaoId { get; set; }

    public Medicamento ToDomain() => new(Nome, Descricao, PrescricaoId);
}

public class MedicamentoPatchRequest
{
    [MaxLength(50)]
    public string? Nome { get; set; }

    public string? Descricao { get; set; }
}

public class MedicamentoResponse
{
    public int Id { get; }
    public string Nome { get; }
    public string Descricao { get; }
    public int PrescricaoId { get; }

    public MedicamentoResponse(Medicamento medicamento)
    {
        Id = medicamento.Id;
        Nome = medicamento.Nome;
        Descricao = medicamento.Descricao;
        PrescricaoId = medicamento.PrescricaoId;
    }
}

public class MedicamentoQueryParameters : QueryParameters
{
    public int? PrescricaoId { get; set; }

    public string? Nome { get; set; }

    public IReadOnlyList<Expression<Func<Medicamento, bool>>> ToFilters()
    {
        var prescricaoId = PrescricaoId.GetValueOrDefault();
        var nome = TextFilter.Normalize(Nome);

        return new FilterBuilder<Medicamento>()
            .AddIf(PrescricaoId.HasValue, medicamento => medicamento.PrescricaoId == prescricaoId)
            .AddIf(nome is not null, medicamento => medicamento.Nome.ToUpper().Contains(nome!))
            .Build();
    }
}
