using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Exame
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; private set; }

    [Required]
    public string Descricao { get; private set; }

    public string? Resultado { get; private set; }

    public DateTime Data { get; private set; }

    [ForeignKey(nameof(Consulta))]
    public int ConsultaId { get; private set; }
    public Consulta Consulta { get; set; }

    public Exame(string tipo, string descricao, DateTime data, int consultaId, string? resultado = null)
    {
        UpdateTipo(tipo);
        UpdateDescricao(descricao);
        Data = data;
        ConsultaId = consultaId;
        Resultado = resultado;
    }

    public void UpdateTipo(string tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo))
            throw new ArgumentException("Tipo de exame é obrigatório");
        Tipo = tipo.Trim();
    }

    public void UpdateDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");
        Descricao = descricao.Trim();
    }

    public void UpdateResultado(string? resultado) => Resultado = resultado?.Trim();

    public override string ToString() => $"Exame: {Tipo} - {Data:dd/MM/yyyy}";

    public Exame() { }
}