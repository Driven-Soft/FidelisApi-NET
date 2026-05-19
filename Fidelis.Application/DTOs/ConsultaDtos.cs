using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class ConsultaRequest
{
    [Required]
    public DateTime DataHora { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    public string? Diagnostico { get; set; }

    public string? Observacoes { get; set; }

    public DateTime? DataRetorno { get; set; }

    [Required]
    public int VeterinarioId { get; set; }

    [Required]
    public int PetId { get; set; }

    public Consulta ToDomain() => new(DataHora, Tipo, VeterinarioId, PetId, Diagnostico, Observacoes, DataRetorno);
}

public class ConsultaPatchRequest
{
    [MaxLength(50)]
    public string? Tipo { get; set; }

    public string? Diagnostico { get; set; }

    public string? Observacoes { get; set; }

    public DateTime? DataRetorno { get; set; }
}

public class ConsultaResponse
{
    public int Id { get; }
    public DateTime DataHora { get; }
    public string Tipo { get; }
    public string? Diagnostico { get; }
    public string? Observacoes { get; }
    public DateTime? DataRetorno { get; }
    public int VeterinarioId { get; }
    public int PetId { get; }

    public ConsultaResponse(Consulta consulta)
    {
        Id = consulta.Id;
        DataHora = consulta.DataHora;
        Tipo = consulta.Tipo;
        Diagnostico = consulta.Diagnostico;
        Observacoes = consulta.Observacoes;
        DataRetorno = consulta.DataRetorno;
        VeterinarioId = consulta.VeterinarioId;
        PetId = consulta.PetId;
    }
}

