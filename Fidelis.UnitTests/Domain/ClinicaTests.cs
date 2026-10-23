using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class ClinicaTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaClinicaComCamposNormalizados()
    {
        var clinica = new Clinica("  Clínica Vida  ", "12.345.678/0001-90", "1133334444", "contato@vida.com", "Av. Paulista, 1000");

        Assert.Equal("Clínica Vida", clinica.Nome);
        Assert.Equal("12.345.678/0001-90", clinica.Cnpj);
        Assert.Equal("contato@vida.com", clinica.Email);
    }

    [Theory]
    [InlineData("", "12345678000190", "1133334444", "a@a.com", "Rua A")]
    [InlineData("Clínica", " ", "1133334444", "a@a.com", "Rua A")]
    [InlineData("Clínica", "12345678000190", "", "a@a.com", "Rua A")]
    [InlineData("Clínica", "12345678000190", "1133334444", "", "Rua A")]
    [InlineData("Clínica", "12345678000190", "1133334444", "a@a.com", "")]
    public void Constructor_CampoObrigatorioVazio_LancaArgumentException(string nome, string cnpj, string telefone, string email, string endereco)
    {
        var act = () => new Clinica(nome, cnpj, telefone, email, endereco);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Update_DadosValidos_AtualizaTodosOsCampos()
    {
        var clinica = new Clinica("Clínica A", "111", "1111", "a@a.com", "Rua A");

        clinica.Update("Clínica B", "222", "2222", "b@b.com", "Rua B");

        Assert.Equal("Clínica B", clinica.Nome);
        Assert.Equal("222", clinica.Cnpj);
        Assert.Equal("2222", clinica.Telefone);
        Assert.Equal("b@b.com", clinica.Email);
        Assert.Equal("Rua B", clinica.Endereco);
    }
}
