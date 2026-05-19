using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class TutorRequest
{
    [Required]
    [MaxLength(14)]
    public string Cpf { get; set; } = string.Empty;

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
    [MaxLength(15)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Endereco { get; set; } = string.Empty;

    public Tutor ToDomain() => new(Cpf, Nome, Email, Senha, Telefone, Endereco);
}

public class TutorPatchRequest
{
    [MaxLength(14)]
    public string? Cpf { get; set; }

    [MaxLength(75)]
    public string? Nome { get; set; }

    [MaxLength(75)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(15)]
    public string? Telefone { get; set; }

    [MaxLength(255)]
    public string? Endereco { get; set; }
}

public class TutorResponse
{
    public int Id { get; }
    public string Cpf { get; }
    public string Nome { get; }
    public string Email { get; }
    public string Telefone { get; }
    public string Endereco { get; }
    public DateTime DataCriacao { get; }

    public TutorResponse(Tutor tutor)
    {
        Id = tutor.Id;
        Cpf = tutor.Cpf;
        Nome = tutor.Nome;
        Email = tutor.Email;
        Telefone = tutor.Telefone;
        Endereco = tutor.Endereco;
        DataCriacao = tutor.DataCriacao;
    }
}

