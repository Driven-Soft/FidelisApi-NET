using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Fidelis.Application.Common;
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

public class LembreteQueryParameters : QueryParameters
{
    public int? PetId { get; set; }

    public int? TutorId { get; set; }

    public char? Status { get; set; }

    public string? Tipo { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public IReadOnlyList<Expression<Func<Lembrete, bool>>> ToFilters()
    {
        var petId = PetId.GetValueOrDefault();
        var tutorId = TutorId.GetValueOrDefault();
        var status = char.ToUpperInvariant(Status.GetValueOrDefault());
        var tipo = TextFilter.Normalize(Tipo);
        var dataInicio = DataInicio.GetValueOrDefault();
        var dataFim = DataFim.GetValueOrDefault();

        return new FilterBuilder<Lembrete>()
            .AddIf(PetId.HasValue, lembrete => lembrete.PetId == petId)
            .AddIf(TutorId.HasValue, lembrete => lembrete.TutorId == tutorId)
            .AddIf(Status.HasValue, lembrete => lembrete.Status == status)
            .AddIf(tipo is not null, lembrete => lembrete.Tipo.ToUpper().Contains(tipo!))
            .AddIf(DataInicio.HasValue, lembrete => lembrete.DataPrevista >= dataInicio)
            .AddIf(DataFim.HasValue, lembrete => lembrete.DataPrevista <= dataFim)
            .Build();
    }
}
