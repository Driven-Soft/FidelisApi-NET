using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class PetTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaPetComStatusAtivo()
    {
        // Arrange
        var nome = "Rex";
        var especie = "Cachorro";
        var raca = "Labrador";
        var sexo = 'M';
        var dataNascimento = new DateTime(2020, 1, 1);
        var fotoUrl = "https://example.com/rex.jpg";
        var tutorId = 1;

        // Act
        var pet = new Pet(nome, especie, raca, sexo, dataNascimento, fotoUrl, tutorId);

        // Assert
        Assert.Equal(nome, pet.Nome);
        Assert.Equal(especie, pet.Especie);
        Assert.Equal(raca, pet.Raca);
        Assert.Equal(sexo, pet.Sexo);
        Assert.Equal('A', pet.Status);
        Assert.Equal(tutorId, pet.TutorId);
    }

    [Fact]
    public void Constructor_NomeVazio_LancaArgumentException()
    {
        // Arrange
        var nomeInvalido = "   ";

        // Act
        var act = () => new Pet(nomeInvalido, "Cachorro", "SRD", 'M', DateTime.UtcNow, "foto.jpg", 1);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Theory]
    [InlineData('X')]
    [InlineData('Z')]
    public void UpdateSexo_ValorForaDoPermitido_LancaArgumentException(char sexoInvalido)
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "SRD", 'M', DateTime.UtcNow, "foto.jpg", 1);

        // Act
        var act = () => pet.UpdateSexo(sexoInvalido);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Inativar_PetAtivo_AlteraStatusParaInativo()
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "SRD", 'M', DateTime.UtcNow, "foto.jpg", 1);

        // Act
        pet.Inativar();

        // Assert
        Assert.Equal('I', pet.Status);
    }

    [Fact]
    public void Update_DadosValidos_AtualizaCamposDoPet()
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "SRD", 'M', new DateTime(2020, 1, 1), "foto-antiga.jpg", 1);
        var novaDataNascimento = new DateTime(2021, 5, 10);

        // Act
        pet.Update("Rex Atualizado", "Gato", "Persa", 'F', novaDataNascimento, "foto-nova.jpg");

        // Assert
        Assert.Equal("Rex Atualizado", pet.Nome);
        Assert.Equal("Gato", pet.Especie);
        Assert.Equal("Persa", pet.Raca);
        Assert.Equal('F', pet.Sexo);
        Assert.Equal(novaDataNascimento, pet.DataNascimento);
        Assert.Equal("foto-nova.jpg", pet.FotoUrl);
    }
}
