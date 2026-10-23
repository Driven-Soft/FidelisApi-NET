using System.ComponentModel.DataAnnotations;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class RegistroDiarioRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int PetId { get; set; }

    [Required]
    [MaxLength(RegistroDiario.MaxTituloLength)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(RegistroDiario.MaxDescricaoLength)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public string Humor { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    [Required]
    public DateTime DataRegistro { get; set; }

    public RegistroDiario ToDomain() => new(PetId, Titulo, Descricao, Humor, Tags, DataRegistro);
}

public class RegistroDiarioUpdateRequest
{
    [Required]
    [MaxLength(RegistroDiario.MaxTituloLength)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [MaxLength(RegistroDiario.MaxDescricaoLength)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public string Humor { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    [Required]
    public DateTime DataRegistro { get; set; }
}

public class RegistroDiarioPatchRequest
{
    [MaxLength(RegistroDiario.MaxTituloLength)]
    public string? Titulo { get; set; }

    [MaxLength(RegistroDiario.MaxDescricaoLength)]
    public string? Descricao { get; set; }

    public string? Humor { get; set; }

    public List<string>? Tags { get; set; }

    public DateTime? DataRegistro { get; set; }
}

public class RegistroDiarioResponse
{
    public string Id { get; }
    public int PetId { get; }
    public string Titulo { get; }
    public string Descricao { get; }
    public string Humor { get; }
    public IReadOnlyList<string> Tags { get; }
    public DateTime DataRegistro { get; }
    public DateTime CriadoEm { get; }
    public DateTime? AtualizadoEm { get; }

    public RegistroDiarioResponse(RegistroDiario registro)
    {
        Id = registro.Id;
        PetId = registro.PetId;
        Titulo = registro.Titulo;
        Descricao = registro.Descricao;
        Humor = registro.Humor;
        Tags = registro.Tags.ToList();
        DataRegistro = registro.DataRegistro;
        CriadoEm = registro.CriadoEm;
        AtualizadoEm = registro.AtualizadoEm;
    }
}

public class RegistroDiarioQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public string? Humor { get; set; }

    public string? Tag { get; set; }

    public string? Texto { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }
}
