using System.ComponentModel.DataAnnotations;
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

