using System.ComponentModel.DataAnnotations;
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

