using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Recomendacao
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; private set; }

    [Required]
    public string Descricao { get; private set; }

    public DateTime DataRecomendacao { get; private set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    public Recomendacao(string tipo, string descricao, DateTime dataRecomendacao, int petId)
    {
        UpdateTipo(tipo);
        UpdateDescricao(descricao);
        DataRecomendacao = dataRecomendacao;
        PetId = petId;
    }

    public void UpdateTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo é obrigatório");
        Tipo = tipo.Trim();
    }

    public void UpdateDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");
        Descricao = descricao.Trim();
    }

    public override string ToString() => $"Recomendação: {Tipo} - {DataRecomendacao:dd/MM/yyyy}";

    public Recomendacao() { }
}