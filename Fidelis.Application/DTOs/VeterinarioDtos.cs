using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class VeterinarioRequest
{
    [Required]
    [MaxLength(13)]
    public string Crmv { get; set; } = string.Empty;

    [Required]
    [MaxLength(75)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(75)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Especialidade { get; set; } = string.Empty;

    [Required]
    public int ClinicaId { get; set; }

    public Veterinario ToDomain() => new(Crmv, Nome, Email, Senha, Especialidade, ClinicaId);
}

public class VeterinarioPatchRequest
{
    [MaxLength(13)]
    public string? Crmv { get; set; }

    [MaxLength(75)]
    public string? Nome { get; set; }

    [MaxLength(75)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Especialidade { get; set; }
}

public class VeterinarioResponse
{
    public int Id { get; }
    public string Crmv { get; }
    public string Nome { get; }
    public string Email { get; }
    public string Especialidade { get; }
    public int ClinicaId { get; }
    public DateTime DataCriacao { get; }

    public VeterinarioResponse(Veterinario veterinario)
    {
        Id = veterinario.Id;
        Crmv = veterinario.Crmv;
        Nome = veterinario.Nome;
        Email = veterinario.Email;
        Especialidade = veterinario.Especialidade;
        ClinicaId = veterinario.ClinicaId;
        DataCriacao = veterinario.DataCriacao;
    }
}

