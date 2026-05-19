using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Lembrete
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; private set; }

    [Required]
    public string Descricao { get; private set; }

    public DateTime DataPrevista { get; private set; }

    public char Status { get; private set; }

    [ForeignKey(nameof(Tutor))]
    public int TutorId { get; private set; }
    public Tutor Tutor { get; set; }

    [ForeignKey(nameof(Pet))]
    public int PetId { get; private set; }
    public Pet Pet { get; set; }

    public Lembrete(string tipo, string descricao, DateTime dataPrevista, int tutorId, int petId)
    {
        UpdateTipo(tipo);
        UpdateDescricao(descricao);
        DataPrevista = dataPrevista;
        TutorId = tutorId;
        PetId = petId;
        Status = 'P'; // Pendente
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

    public void Concluir() => Status = 'C';
    public void Cancelar() => Status = 'X';

    public override string ToString() => $"Lembrete: {Tipo} - {DataPrevista:dd/MM/yyyy} [{Status}]";

    public Lembrete() { }
}