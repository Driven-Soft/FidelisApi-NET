using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Vermifugacao
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Produto { get; private set; }

    public DateTime DataAplicacao { get; private set; }

    public DateTime DataProxima { get; private set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    [ForeignKey(nameof(Veterinario))]
    public int VeterinarioId { get; private set; }
    public Veterinario Veterinario { get; set; }

    public Vermifugacao(string produto, DateTime dataAplicacao, DateTime dataProxima, int petId, int veterinarioId)
    {
        UpdateProduto(produto);
        UpdateDatas(dataAplicacao, dataProxima);
        PetId = petId;
        VeterinarioId = veterinarioId;
    }

    public void UpdateProduto(string produto)
    {
        if (string.IsNullOrWhiteSpace(produto))
            throw new ArgumentException("Produto é obrigatório");
        Produto = produto.Trim();
    }

    public void UpdateDatas(DateTime dataAplicacao, DateTime dataProxima)
    {
        if (dataProxima <= dataAplicacao)
            throw new ArgumentException("Data da próxima aplicação deve ser posterior à data de aplicação");
        DataAplicacao = dataAplicacao;
        DataProxima = dataProxima;
    }

    public override string ToString() => $"Vermifugação: {Produto} - {DataAplicacao:dd/MM/yyyy}";

    public Vermifugacao() { }
}