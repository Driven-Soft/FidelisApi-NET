using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class LembreteRequest
{
    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public DateTime DataPrevista { get; set; }

    [Required]
    public int TutorId { get; set; }

    [Required]
    public int PetId { get; set; }

    public Lembrete ToDomain() => new(Tipo, Descricao, DataPrevista, TutorId, PetId);
}

public class LembretePatchRequest
{
    [MaxLength(50)]
    public string? Tipo { get; set; }

    public string? Descricao { get; set; }

    public char? Status { get; set; }
}

public class LembreteResponse
{
    public int Id { get; }
    public string Tipo { get; }
    public string Descricao { get; }
    public DateTime DataPrevista { get; }
    public char Status { get; }
    public int TutorId { get; }
    public int PetId { get; }

    public LembreteResponse(Lembrete lembrete)
    {
        Id = lembrete.Id;
        Tipo = lembrete.Tipo;
        Descricao = lembrete.Descricao;
        DataPrevista = lembrete.DataPrevista;
        Status = lembrete.Status;
        TutorId = lembrete.TutorId;
        PetId = lembrete.PetId;
    }
}

