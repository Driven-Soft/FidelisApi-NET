using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class VermifugacaoRequest
{
    [Required]
    [MaxLength(50)]
    public string Produto { get; set; } = string.Empty;

    [Required]
    public DateTime DataAplicacao { get; set; }

    [Required]
    public DateTime DataProxima { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    public int VeterinarioId { get; set; }

    public Vermifugacao ToDomain() => new(Produto, DataAplicacao, DataProxima, PetId, VeterinarioId);
}

public class VermifugacaoPatchRequest
{
    [MaxLength(50)]
    public string? Produto { get; set; }

    public DateTime? DataAplicacao { get; set; }

    public DateTime? DataProxima { get; set; }
}

public class VermifugacaoResponse
{
    public int Id { get; }
    public string Produto { get; }
    public DateTime DataAplicacao { get; }
    public DateTime DataProxima { get; }
    public int PetId { get; }
    public int VeterinarioId { get; }

    public VermifugacaoResponse(Vermifugacao vermifugacao)
    {
        Id = vermifugacao.Id;
        Produto = vermifugacao.Produto;
        DataAplicacao = vermifugacao.DataAplicacao;
        DataProxima = vermifugacao.DataProxima;
        PetId = vermifugacao.PetId;
        VeterinarioId = vermifugacao.VeterinarioId;
    }
}

public class VermifugacaoQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public int? VeterinarioId { get; set; }

    public string? Produto { get; set; }

    public DateTime? ProximaAte { get; set; }

    public IReadOnlyList<Expression<Func<Vermifugacao, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var veterinarioId = VeterinarioId.GetValueOrDefault();
        var produto = TextFilter.Normalize(Produto);
        var proximaAte = ProximaAte.GetValueOrDefault();

        return new FilterBuilder<Vermifugacao>()
            .AddIf(PetId.HasValue, vermifugacao => vermifugacao.PetId == petId)
            .AddIf(VeterinarioId.HasValue, vermifugacao => vermifugacao.VeterinarioId == veterinarioId)
            .AddIf(produto is not null, vermifugacao => vermifugacao.Produto.ToUpper().Contains(produto!))
            .AddIf(ProximaAte.HasValue, vermifugacao => vermifugacao.DataProxima <= proximaAte)
            .Build();
    }
}
