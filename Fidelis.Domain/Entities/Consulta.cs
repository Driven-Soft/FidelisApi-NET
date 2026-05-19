using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Consulta
{
    [Key]
    public int Id { get; private set; }

    public DateTime DataHora { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; private set; }

    public string? Diagnostico { get; private set; }

    public string? Observacoes { get; private set; }

    public DateTime? DataRetorno { get; private set; }

    [ForeignKey(nameof(Veterinario))]
    public int VeterinarioId { get; private set; }
    public Veterinario Veterinario { get; set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    public List<Exame> Exames { get; set; } = [];
    public List<Prescricao> Prescricoes { get; set; } = [];

    public Consulta(DateTime dataHora, string tipo, int veterinarioId, int petId, string? diagnostico = null, string? observacoes = null, DateTime? dataRetorno = null)
    {
        UpdateTipo(tipo);
        DataHora = dataHora;
        VeterinarioId = veterinarioId;
        PetId = petId;
        Diagnostico = diagnostico;
        Observacoes = observacoes;
        DataRetorno = dataRetorno;
    }

    public void UpdateTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo de consulta é obrigatório");
        Tipo = tipo.Trim();
    }

    public void UpdateDiagnostico(string? diagnostico) => Diagnostico = diagnostico?.Trim();

    public void UpdateObservacoes(string? observacoes) => Observacoes = observacoes?.Trim();

    public void UpdateDataRetorno(DateTime? dataRetorno) => DataRetorno = dataRetorno;

    public void Update(string tipo, string? diagnostico, string? observacoes, DateTime? dataRetorno)
    {
        UpdateTipo(tipo);
        UpdateDiagnostico(diagnostico);
        UpdateObservacoes(observacoes);
        UpdateDataRetorno(dataRetorno);
    }

    public override string ToString() => $"Consulta: {Tipo} - {DataHora:dd/MM/yyyy HH:mm}";

    public Consulta() { }
}