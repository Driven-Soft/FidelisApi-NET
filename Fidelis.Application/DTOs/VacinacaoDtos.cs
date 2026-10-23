using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class VacinacaoRequest
{
    [Required]
    [MaxLength(50)]
    public string VacinaAplicada { get; set; } = string.Empty;

    [Required]
    public DateTime DataAplicacao { get; set; }

    [Required]
    public DateTime DataProxima { get; set; }

    public string? Observacao { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    public int VeterinarioId { get; set; }

    public Vacinacao ToDomain() => new(VacinaAplicada, DataAplicacao, DataProxima, PetId, VeterinarioId, Observacao);
}

public class VacinacaoPatchRequest
{
    [MaxLength(50)]
    public string? VacinaAplicada { get; set; }

    public DateTime? DataAplicacao { get; set; }

    public DateTime? DataProxima { get; set; }
}

public class VacinacaoResponse
{
    public int Id { get; }
    public DateTime DataAplicacao { get; }
    public DateTime DataProxima { get; }
    public string VacinaAplicada { get; }
    public string? Observacao { get; }
    public int PetId { get; }
    public int VeterinarioId { get; }

    public VacinacaoResponse(Vacinacao vacinacao)
    {
        Id = vacinacao.Id;
        DataAplicacao = vacinacao.DataAplicacao;
        DataProxima = vacinacao.DataProxima;
        VacinaAplicada = vacinacao.VacinaAplicada;
        Observacao = vacinacao.Observacao;
        PetId = vacinacao.PetId;
        VeterinarioId = vacinacao.VeterinarioId;
    }
}

public class VacinacaoQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public int? VeterinarioId { get; set; }

    public string? Vacina { get; set; }

    public DateTime? ProximaAte { get; set; }

    public IReadOnlyList<Expression<Func<Vacinacao, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var veterinarioId = VeterinarioId.GetValueOrDefault();
        var vacina = TextFilter.Normalize(Vacina);
        var proximaAte = ProximaAte.GetValueOrDefault();

        return new FilterBuilder<Vacinacao>()
            .AddIf(PetId.HasValue, vacinacao => vacinacao.PetId == petId)
            .AddIf(VeterinarioId.HasValue, vacinacao => vacinacao.VeterinarioId == veterinarioId)
            .AddIf(vacina is not null, vacinacao => vacinacao.VacinaAplicada.ToUpper().Contains(vacina!))
            .AddIf(ProximaAte.HasValue, vacinacao => vacinacao.DataProxima <= proximaAte)
            .Build();
    }
}
