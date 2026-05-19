using System.ComponentModel.DataAnnotations;
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

