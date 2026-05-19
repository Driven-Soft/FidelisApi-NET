using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class ExameRequest
{
    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime Data { get; set; }

    public string? Resultado { get; set; }

    [Required]
    public int ConsultaId { get; set; }

    public Exame ToDomain() => new(Tipo, Descricao, Data, ConsultaId, Resultado);
}

public class ExamePatchRequest
{
    [MaxLength(50)]
    public string? Tipo { get; set; }

    public string? Descricao { get; set; }

    public string? Resultado { get; set; }
}

public class ExameResponse
{
    public int Id { get; }
    public string Tipo { get; }
    public string Descricao { get; }
    public string? Resultado { get; }
    public DateTime Data { get; }
    public int ConsultaId { get; }

    public ExameResponse(Exame exame)
    {
        Id = exame.Id;
        Tipo = exame.Tipo;
        Descricao = exame.Descricao;
        Resultado = exame.Resultado;
        Data = exame.Data;
        ConsultaId = exame.ConsultaId;
    }
}

