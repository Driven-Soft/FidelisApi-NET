using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Comportamento
{
    [Key]
    public int Id { get; private set; }

    public DateTime Data { get; private set; }

    [Required]
    public string Descricao { get; private set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    public Comportamento(string descricao, DateTime data, int petId)
    {
        UpdateDescricao(descricao);
        Data = data;
        PetId = petId;
    }

    public void UpdateDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");
        Descricao = descricao.Trim();
    }

    public override string ToString() => $"Comportamento registrado em {Data:dd/MM/yyyy}";

    public Comportamento() { }
}