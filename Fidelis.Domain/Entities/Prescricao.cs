using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Prescricao
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Dosagem { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Frequencia { get; private set; }

    public int DuracaoDias { get; private set; }

    public string? Observacao { get; private set; }

    [ForeignKey(nameof(Consulta))]
    public int ConsultaId { get; private set; }
    public Consulta Consulta { get; set; }

    public List<Medicamento> Medicamentos { get; set; } = [];

    public Prescricao(string dosagem, string frequencia, int duracaoDias, int consultaId, string? observacao = null)
    {
        UpdateDosagem(dosagem);
        UpdateFrequencia(frequencia);
        UpdateDuracaoDias(duracaoDias);
        ConsultaId = consultaId;
        Observacao = observacao;
    }

    public void UpdateDosagem(string dosagem)
    {
        if (string.IsNullOrWhiteSpace(dosagem))
            throw new ArgumentException("Dosagem é obrigatória");
        Dosagem = dosagem.Trim();
    }

    public void UpdateFrequencia(string frequencia)
    {
        if (string.IsNullOrWhiteSpace(frequencia))
            throw new ArgumentException("Frequência é obrigatória");
        Frequencia = frequencia.Trim();
    }

    public void UpdateDuracaoDias(int duracaoDias)
    {
        if (duracaoDias <= 0)
            throw new ArgumentException("Duração deve ser maior que zero");
        DuracaoDias = duracaoDias;
    }

    public void Update(string dosagem, string frequencia, int duracaoDias, string? observacao)
    {
        UpdateDosagem(dosagem);
        UpdateFrequencia(frequencia);
        UpdateDuracaoDias(duracaoDias);
        Observacao = observacao;
    }

    public override string ToString() => $"Prescrição: {Dosagem} - {Frequencia} por {DuracaoDias} dias";

    public Prescricao() { }
}