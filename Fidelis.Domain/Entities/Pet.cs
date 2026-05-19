using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Pet
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(30)]
    public string Nome { get; private set; }

    [Required]
    [MaxLength(20)]
    public string Especie { get; private set; }

    [Required]
    [MaxLength(20)]
    public string Raca { get; private set; }

    [Required]
    public char Sexo { get; private set; }

    public DateTime DataNascimento { get; private set; }

    public char Status { get; private set; }

    [Required]
    [MaxLength(255)]
    public string FotoUrl { get; private set; }

    [ForeignKey(nameof(Tutor))]
    public int TutorId { get; private set; }
    public Tutor Tutor { get; set; }

    [ForeignKey(nameof(Clinica))]
    public int? ClinicaId { get; private set; }
    public Clinica? Clinica { get; set; }

    public List<Consulta> Consultas { get; set; } = [];
    public List<Vacinacao> Vacinacoes { get; set; } = [];
    public List<Vermifugacao> Vermifugacoes { get; set; } = [];
    public List<HistoricoPeso> HistoricoPesos { get; set; } = [];
    public List<Comportamento> Comportamentos { get; set; } = [];
    public List<Recomendacao> Recomendacoes { get; set; } = [];
    public List<Lembrete> Lembretes { get; set; } = [];

    public Pet(string nome, string especie, string raca, char sexo, DateTime dataNascimento, string fotoUrl, int tutorId, int? clinicaId = null)
    {
        UpdateNome(nome);
        UpdateEspecie(especie);
        UpdateRaca(raca);
        UpdateSexo(sexo);
        DataNascimento = dataNascimento;
        FotoUrl = fotoUrl;
        TutorId = tutorId;
        ClinicaId = clinicaId;
        Status = 'A';
    }

    public void UpdateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório");
        Nome = nome.Trim();
    }

    public void UpdateEspecie(string especie)
    {
        if (string.IsNullOrWhiteSpace(especie))
            throw new ArgumentException("Espécie é obrigatória");
        Especie = especie.Trim();
    }

    public void UpdateRaca(string raca)
    {
        if (string.IsNullOrWhiteSpace(raca))
            throw new ArgumentException("Raça é obrigatória");
        Raca = raca.Trim();
    }

    public void UpdateSexo(char sexo)
    {
        if (sexo != 'M' && sexo != 'F')
            throw new ArgumentException("Sexo deve ser 'M' ou 'F'");
        Sexo = sexo;
    }

    public void Inativar() => Status = 'I';
    public void Ativar() => Status = 'A';

    public void Update(string nome, string especie, string raca, char sexo, DateTime dataNascimento, string fotoUrl)
    {
        UpdateNome(nome);
        UpdateEspecie(especie);
        UpdateRaca(raca);
        UpdateSexo(sexo);
        DataNascimento = dataNascimento;
        FotoUrl = fotoUrl;
    }

    public override string ToString() => $"Pet: {Nome} - {Especie} ({Raca})";

    public Pet() { }
}