using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class PetRequest
{
    [Required]
    [MaxLength(30)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Especie { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Raca { get; set; } = string.Empty;

    [Required]
    public char Sexo { get; set; }

    [Required]
    public DateTime DataNascimento { get; set; }

    [Required]
    [MaxLength(255)]
    public string FotoUrl { get; set; } = string.Empty;

    [Required]
    public int TutorId { get; set; }

    public int? ClinicaId { get; set; }

    public Pet ToDomain() => new(Nome, Especie, Raca, Sexo, DataNascimento, FotoUrl, TutorId, ClinicaId);
}

public class PetPatchRequest
{
    [MaxLength(30)]
    public string? Nome { get; set; }

    [MaxLength(20)]
    public string? Especie { get; set; }

    [MaxLength(20)]
    public string? Raca { get; set; }

    public char? Sexo { get; set; }

    public DateTime? DataNascimento { get; set; }

    [MaxLength(255)]
    public string? FotoUrl { get; set; }

    public int? ClinicaId { get; set; }
}

public class PetResponse
{
    public int Id { get; }
    public string Nome { get; }
    public string Especie { get; }
    public string Raca { get; }
    public char Sexo { get; }
    public DateTime DataNascimento { get; }
    public char Status { get; }
    public string FotoUrl { get; }
    public int TutorId { get; }
    public int? ClinicaId { get; }

    public PetResponse(Pet pet)
    {
        Id = pet.Id;
        Nome = pet.Nome;
        Especie = pet.Especie;
        Raca = pet.Raca;
        Sexo = pet.Sexo;
        DataNascimento = pet.DataNascimento;
        Status = pet.Status;
        FotoUrl = pet.FotoUrl;
        TutorId = pet.TutorId;
        ClinicaId = pet.ClinicaId;
    }
}

public class PetQueryParameters : QueryParameters
{
    public string? Nome { get; set; }

    public string? Especie { get; set; }

    public string? Raca { get; set; }

    public char? Sexo { get; set; }

    public char? Status { get; set; }

    public int? TutorId { get; set; }

    public int? ClinicaId { get; set; }

    public IReadOnlyList<Expression<Func<Pet, bool>>> ToFilters()
    {
        var nome = TextFilter.Normalize(Nome);
        var especie = TextFilter.Normalize(Especie);
        var raca = TextFilter.Normalize(Raca);
        var sexo = char.ToUpperInvariant(Sexo.GetValueOrDefault());
        var status = char.ToUpperInvariant(Status.GetValueOrDefault());
        var tutorId = TutorId.GetValueOrDefault();
        var clinicaId = ClinicaId.GetValueOrDefault();

        return new FilterBuilder<Pet>()
            .AddIf(nome is not null, pet => pet.Nome.ToUpper().Contains(nome!))
            .AddIf(especie is not null, pet => pet.Especie.ToUpper().Contains(especie!))
            .AddIf(raca is not null, pet => pet.Raca.ToUpper().Contains(raca!))
            .AddIf(Sexo.HasValue, pet => pet.Sexo == sexo)
            .AddIf(Status.HasValue, pet => pet.Status == status)
            .AddIf(TutorId.HasValue, pet => pet.TutorId == tutorId)
            .AddIf(ClinicaId.HasValue, pet => pet.ClinicaId == clinicaId)
            .Build();
    }
}
