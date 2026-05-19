using System.ComponentModel.DataAnnotations;
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

