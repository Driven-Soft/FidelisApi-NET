using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Medicamento
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; private set; }

    [Required]
    public string Descricao { get; private set; }

    [ForeignKey(nameof(Prescricao))]
    public int PrescricaoId { get; private set; }
    public Prescricao Prescricao { get; set; }

    public Medicamento(string nome, string descricao, int prescricaoId)
    {
        UpdateNome(nome);
        UpdateDescricao(descricao);
        PrescricaoId = prescricaoId;
    }

    public void UpdateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório");
        Nome = nome.Trim();
    }

    public void UpdateDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");
        Descricao = descricao.Trim();
    }

    public void Update(string nome, string descricao)
    {
        UpdateNome(nome);
        UpdateDescricao(descricao);
    }

    public override string ToString() => $"Medicamento: {Nome}";

    public Medicamento() { }
}