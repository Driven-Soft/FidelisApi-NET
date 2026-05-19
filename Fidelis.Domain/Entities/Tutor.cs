using System.ComponentModel.DataAnnotations;

namespace Fidelis.Domain.Entities;

public class Tutor
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(14)]
    public string Cpf { get; private set; }

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
    [MaxLength(15)]
    public string Telefone { get; private set; }

    [Required]
    [MaxLength(255)]
    public string Endereco { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public List<Pet> Pets { get; set; } = [];
    public List<Lembrete> Lembretes { get; set; } = [];

    public Tutor(string cpf, string nome, string email, string senha, string telefone, string endereco)
    {
        UpdateCpf(cpf);
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateSenha(senha);
        UpdateTelefone(telefone);
        UpdateEndereco(endereco);
        DataCriacao = DateTime.UtcNow;
    }

    public void UpdateCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF é obrigatório");
        Cpf = cpf.Trim();
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

    public void UpdateTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório");
        Telefone = telefone.Trim();
    }

    public void UpdateEndereco(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("Endereço é obrigatório");
        Endereco = endereco.Trim();
    }

    public void Update(string cpf, string nome, string email, string telefone, string endereco)
    {
        UpdateCpf(cpf);
        UpdateNome(nome);
        UpdateEmail(email);
        UpdateTelefone(telefone);
        UpdateEndereco(endereco);
    }

    public override string ToString() => $"Tutor: {Nome} - CPF: {Cpf}";

    public Tutor() { }
}