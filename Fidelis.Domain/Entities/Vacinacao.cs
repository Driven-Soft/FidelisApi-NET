using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Vacinacao
{
    [Key]
    public int Id { get; private set; }

    public DateTime DataAplicacao { get; private set; }

    public DateTime DataProxima { get; private set; }

    [Required]
    [MaxLength(50)]
    public string VacinaAplicada { get; private set; }

    public string? Observacao { get; private set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    [ForeignKey(nameof(Veterinario))]
    public int VeterinarioId { get; private set; }
    public Veterinario Veterinario { get; set; }

    public Vacinacao(string vacinaAplicada, DateTime dataAplicacao, DateTime dataProxima, int petId, int veterinarioId, string? observacao = null)
    {
        UpdateVacinaAplicada(vacinaAplicada);
        UpdateDatas(dataAplicacao, dataProxima);
        PetId = petId;
        VeterinarioId = veterinarioId;
        Observacao = observacao;
    }

    public void UpdateVacinaAplicada(string vacina)
    {
        if (string.IsNullOrWhiteSpace(vacina))
            throw new ArgumentException("Vacina aplicada é obrigatória");
        VacinaAplicada = vacina.Trim();
    }

    public void UpdateDatas(DateTime dataAplicacao, DateTime dataProxima)
    {
        if (dataProxima <= dataAplicacao)
            throw new ArgumentException("Data da próxima aplicação deve ser posterior à data de aplicação");
        DataAplicacao = dataAplicacao;
        DataProxima = dataProxima;
    }

    public override string ToString() => $"Vacinação: {VacinaAplicada} - {DataAplicacao:dd/MM/yyyy}";

    public Vacinacao() { }
}