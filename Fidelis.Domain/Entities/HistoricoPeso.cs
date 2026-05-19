using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class HistoricoPeso
{
    [Key]
    public int Id { get; private set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal PesoKg { get; private set; }

    public DateTime DataMedicao { get; private set; }

    public string? Observacao { get; private set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    public HistoricoPeso(decimal pesoKg, DateTime dataMedicao, int petId, string? observacao = null)
    {
        UpdatePeso(pesoKg);
        DataMedicao = dataMedicao;
        PetId = petId;
        Observacao = observacao;
    }

    public void UpdatePeso(decimal pesoKg)
    {
        if (pesoKg <= 0)
            throw new ArgumentException("Peso deve ser maior que zero");
        PesoKg = pesoKg;
    }

    public override string ToString() => $"Peso: {PesoKg}kg em {DataMedicao:dd/MM/yyyy}";

    public HistoricoPeso() { }
}