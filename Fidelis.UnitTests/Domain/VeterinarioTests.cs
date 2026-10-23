using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class VeterinarioTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaVeterinarioComDataDeCriacao()
    {
        var antes = DateTime.UtcNow;

        var veterinario = new Veterinario("SP-12345", "Dra. Ana", "ana@vet.com", "123456", "Dermatologia", 1);

        Assert.Equal("SP-12345", veterinario.Crmv);
        Assert.Equal("Dermatologia", veterinario.Especialidade);
        Assert.Equal(1, veterinario.ClinicaId);
        Assert.True(veterinario.DataCriacao >= antes);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Constructor_SenhaInvalida_LancaArgumentException(string senha)
    {
        var act = () => new Veterinario("SP-12345", "Dra. Ana", "ana@vet.com", senha, "Clínico Geral", 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Update_DadosValidos_NaoAlteraSenha()
    {
        var veterinario = new Veterinario("SP-1", "Dr. João", "joao@vet.com", "senha123", "Clínico Geral", 1);

        veterinario.Update("SP-2", "Dr. João Silva", "joao.silva@vet.com", "Ortopedia");

        Assert.Equal("SP-2", veterinario.Crmv);
        Assert.Equal("Dr. João Silva", veterinario.Nome);
        Assert.Equal("Ortopedia", veterinario.Especialidade);
        Assert.Equal("senha123", veterinario.Senha);
    }

    [Fact]
    public void UpdateEspecialidade_Vazia_LancaArgumentException()
    {
        var veterinario = new Veterinario("SP-1", "Dr. João", "joao@vet.com", "senha123", "Clínico Geral", 1);

        var act = () => veterinario.UpdateEspecialidade(" ");

        Assert.Throws<ArgumentException>(act);
    }
}
