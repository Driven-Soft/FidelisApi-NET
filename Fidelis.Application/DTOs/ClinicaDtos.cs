using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class ClinicaRequest
{
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(18)]
    public string Cnpj { get; set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [MaxLength(75)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Endereco { get; set; } = string.Empty;

    public Clinica ToDomain() => new(Nome, Cnpj, Telefone, Email, Endereco);
}

public class ClinicaPatchRequest
{
    [MaxLength(100)]
    public string? Nome { get; set; }

    [MaxLength(18)]
    public string? Cnpj { get; set; }

    [MaxLength(15)]
    public string? Telefone { get; set; }

    [MaxLength(75)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public string? Endereco { get; set; }
}

public class ClinicaResponse
{
    public int Id { get; }
    public string Nome { get; }
    public string Cnpj { get; }
    public string Telefone { get; }
    public string Email { get; }
    public string Endereco { get; }

    public ClinicaResponse(Clinica clinica)
    {
        Id = clinica.Id;
        Nome = clinica.Nome;
        Cnpj = clinica.Cnpj;
        Telefone = clinica.Telefone;
        Email = clinica.Email;
        Endereco = clinica.Endereco;
    }
}

