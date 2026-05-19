using System.ComponentModel.DataAnnotations;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class VacinacaoRequest
{
    [Required]
    [MaxLength(50)]
    public string VacinaAplicada { get; set; } = string.Empty;

    [Required]
    public DateTime DataAplicacao { get; set; }

    [Required]
    public DateTime DataProxima { get; set; }

    public string? Observacao { get; set; }

    [Required]
    public int PetId { get; set; }

    [Required]
    public int VeterinarioId { get; set; }

    public Vacinacao ToDomain() => new(VacinaAplicada, DataAplicacao, DataProxima, PetId, VeterinarioId, Observacao);
}

public class VacinacaoPatchRequest
{
    [MaxLength(50)]
    public string? VacinaAplicada { get; set; }

    public DateTime? DataAplicacao { get; set; }

    public DateTime? DataProxima { get; set; }
}

public class VacinacaoResponse
{
    public int Id { get; }
    public DateTime DataAplicacao { get; }
    public DateTime DataProxima { get; }
    public string VacinaAplicada { get; }
    public string? Observacao { get; }
    public int PetId { get; }
    public int VeterinarioId { get; }

    public VacinacaoResponse(Vacinacao vacinacao)
    {
        Id = vacinacao.Id;
        DataAplicacao = vacinacao.DataAplicacao;
        DataProxima = vacinacao.DataProxima;
        VacinaAplicada = vacinacao.VacinaAplicada;
        Observacao = vacinacao.Observacao;
        PetId = vacinacao.PetId;
        VeterinarioId = vacinacao.VeterinarioId;
    }
}

