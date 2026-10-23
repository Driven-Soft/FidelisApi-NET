using System.ComponentModel.DataAnnotations;

namespace Fidelis.Application.DTOs;

/// <summary>Dados do tutor enviados ao package Oracle de cadastro.</summary>
public sealed record OracleTutorRequest
{
    /// <summary>Identificador do tutor.</summary>
    [Range(1, int.MaxValue)]
    public int Id { get; init; }

    /// <summary>CPF do tutor.</summary>
    [Required]
    public required string Cpf { get; init; }

    /// <summary>Nome do tutor.</summary>
    [Required]
    public required string Nome { get; init; }

    /// <summary>E-mail do tutor.</summary>
    [Required]
    public required string Email { get; init; }

    /// <summary>Senha do tutor.</summary>
    [Required]
    public required string Senha { get; init; }

    /// <summary>Telefone do tutor.</summary>
    [Required]
    public required string Telefone { get; init; }

    /// <summary>Endereço do tutor.</summary>
    [Required]
    public required string Endereco { get; init; }
}
