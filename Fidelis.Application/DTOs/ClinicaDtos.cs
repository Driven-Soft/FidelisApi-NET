using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
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

public class ClinicaQueryParameters : QueryParameters
{
    public string? Nome { get; set; }

    public string? Cnpj { get; set; }

    public string? Email { get; set; }

    public IReadOnlyList<Expression<Func<Clinica, bool>>> ToFilters()
    {
        var nome = TextFilter.Normalize(Nome);
        var cnpj = Cnpj?.Trim();
        var email = TextFilter.Normalize(Email);

        return new FilterBuilder<Clinica>()
            .AddIf(nome is not null, clinica => clinica.Nome.ToUpper().Contains(nome!))
            .AddIf(!string.IsNullOrWhiteSpace(cnpj), clinica => clinica.Cnpj == cnpj)
            .AddIf(email is not null, clinica => clinica.Email.ToUpper().Contains(email!))
            .Build();
    }
}
