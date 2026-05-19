using System.ComponentModel.DataAnnotations;

namespace Fidelis.Domain.Entities;

public class Clinica
{
    [Key]
    public int Id { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; private set; }

    [Required]
    [MaxLength(18)]
    public string Cnpj { get; private set; }

    [Required]
    [MaxLength(15)]
    public string Telefone { get; private set; }

    [Required]
    [MaxLength(75)]
    public string Email { get; private set; }

    [Required]
    [MaxLength(255)]
    public string Endereco { get; private set; }

    public List<Veterinario> Veterinarios { get; set; } = [];
    public List<Pet> Pets { get; set; } = [];

    public Clinica(string nome, string cnpj, string telefone, string email, string endereco)
    {
        UpdateNome(nome);
        UpdateCnpj(cnpj);
        UpdateTelefone(telefone);
        UpdateEmail(email);
        UpdateEndereco(endereco);
    }

    public void UpdateNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório");
        Nome = nome.Trim();
    }

    public void UpdateCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ é obrigatório");
        Cnpj = cnpj.Trim();
    }

    public void UpdateTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("Telefone é obrigatório");
        Telefone = telefone.Trim();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email é obrigatório");
        Email = email.Trim();
    }

    public void UpdateEndereco(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("Endereço é obrigatório");
        Endereco = endereco.Trim();
    }

    public void Update(string nome, string cnpj, string telefone, string email, string endereco)
    {
        UpdateNome(nome);
        UpdateCnpj(cnpj);
        UpdateTelefone(telefone);
        UpdateEmail(email);
        UpdateEndereco(endereco);
    }

    public override string ToString() => $"Clínica: {Nome} - CNPJ: {Cnpj}";

    public Clinica() { }
}