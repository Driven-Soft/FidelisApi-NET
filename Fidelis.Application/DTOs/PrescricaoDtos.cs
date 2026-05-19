using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class PrescricaoRequest
{
    [Required]
    [MaxLength(50)]
    public string Dosagem { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Frequencia { get; set; } = string.Empty;

    [Required]
    public int DuracaoDias { get; set; }

    public string? Observacao { get; set; }

    [Required]
    public int ConsultaId { get; set; }

    public Prescricao ToDomain() => new(Dosagem, Frequencia, DuracaoDias, ConsultaId, Observacao);
}

public class PrescricaoPatchRequest
{
    [MaxLength(50)]
    public string? Dosagem { get; set; }

    [MaxLength(50)]
    public string? Frequencia { get; set; }

    public int? DuracaoDias { get; set; }

    public string? Observacao { get; set; }
}

public class PrescricaoResponse
{
    public int Id { get; }
    public string Dosagem { get; }
    public string Frequencia { get; }
    public int DuracaoDias { get; }
    public string? Observacao { get; }
    public int ConsultaId { get; }

    public PrescricaoResponse(Prescricao prescricao)
    {
        Id = prescricao.Id;
        Dosagem = prescricao.Dosagem;
        Frequencia = prescricao.Frequencia;
        DuracaoDias = prescricao.DuracaoDias;
        Observacao = prescricao.Observacao;
        ConsultaId = prescricao.ConsultaId;
    }
}
