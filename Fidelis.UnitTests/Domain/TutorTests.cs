using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class TutorTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaTutorComDataDeCriacaoPreenchida()
    {
        // Arrange
        var cpf = "12345678900";
        var nome = "Felipe";
        var email = "felipe@example.com";
        var senha = "123456";
        var telefone = "11988476632";
        var endereco = "Rua Exemplo, 123";

        // Act
        var tutor = new Tutor(cpf, nome, email, senha, telefone, endereco);

        // Assert
        Assert.Equal(cpf, tutor.Cpf);
        Assert.Equal(nome, tutor.Nome);
        Assert.Equal(email, tutor.Email);
        Assert.True(tutor.DataCriacao <= DateTime.UtcNow);
    }

    [Fact]
    public void UpdateSenha_SenhaComMenosDeSeisCaracteres_LancaArgumentException()
    {
        // Arrange
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");

        // Act
        var act = () => tutor.UpdateSenha("123");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateEmail_EmailVazio_LancaArgumentException()
    {
        // Arrange
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");

        // Act
        var act = () => tutor.UpdateEmail(string.Empty);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Update_DadosValidos_AtualizaCamposDoTutorSemAlterarSenha()
    {
        // Arrange
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Antiga");
        var senhaOriginal = tutor.Senha;

        // Act
        tutor.Update("98765432100", "Felipe Atualizado", "novo@example.com", "11999999999", "Rua Nova");

        // Assert
        Assert.Equal("98765432100", tutor.Cpf);
        Assert.Equal("Felipe Atualizado", tutor.Nome);
        Assert.Equal("novo@example.com", tutor.Email);
        Assert.Equal("11999999999", tutor.Telefone);
        Assert.Equal("Rua Nova", tutor.Endereco);
        Assert.Equal(senhaOriginal, tutor.Senha);
    }
}
