using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fidelis.Domain.Entities;

public class Veterinario
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(13)]
    public string Crmv { get; private set; }

    [Required]
    [MaxLength(75)]
    public string Nome { get; private set; }

    [Required]
    [MaxLength(75)]
    public string Email { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Senha { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Especialidade { get; private set; }

    public DateTime DataCriacao { get; private set; }

    [ForeignKey(nameof(Clinica))]
    public int ClinicaId { get; private set; }
    public Clinica Clinica { get; set; }

    public List<Consulta> Consultas { get; set; } = [];
    public List<Vacinacao> Vacinacoes { get; set; } = [];
    public List<Vermifugacao> Vermifugacoes { get; set; } = [];

    public Veterinario(string crmv, string nome, string email, string senha, string especialidade, int clinicaId)
    {
        UpdateCrmv(crmv);
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateSenha(senha);
        UpdateEspecialidade(especialidade);
        ClinicaId = clinicaId;
        DataCriacao = DateTime.UtcNow;
    }

    public void UpdateCrmv(string crmv)
    {
        if (string.IsNullOrWhiteSpace(crmv))
            throw new ArgumentException("CRMV é obrigatório");
        Crmv = crmv.Trim();
    }

    public void UpdateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório");
        Nome = nome.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório");
        Email = email.Trim();
    }

    public void UpdateSenha(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Senha é obrigatória");
        if (senha.Length < 6)
            throw new ArgumentException("Senha deve ter ao menos 6 caracteres");
        Senha = senha;
    }

    public void UpdateEspecialidade(string especialidade)
    {
        if (string.IsNullOrWhiteSpace(especialidade))
            throw new ArgumentException("Especialidade é obrigatória");
        Especialidade = especialidade.Trim();
    }

    public void Update(string crmv, string nome, string email, string especialidade)
    {
        UpdateCrmv(crmv);
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateEspecialidade(especialidade);
    }

    public override string ToString() => $"Veterinário: {Nome} - CRMV: {Crmv} - {Especialidade}";

    public Veterinario() { }
}