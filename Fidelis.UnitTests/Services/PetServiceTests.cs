using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _repositoryMock;
    private readonly PetService _sut; // "sut" = System Under Test

    public PetServiceTests()
    {
        _repositoryMock = new Mock<IPetRepository>();
        _sut = new PetService(_repositoryMock.Object, NullLogger<PetService>.Instance);
    }

    [Fact]
    public void Create_RequestValido_AdicionaPetERetornaResponse()
    {
        // Arrange
        var request = new PetRequest
        {
            Nome = "Rex",
            Especie = "Cachorro",
            Raca = "Labrador",
            Sexo = 'M',
            DataNascimento = new DateTime(2020, 1, 1),
            FotoUrl = "foto.jpg",
            TutorId = 1
        };

        // Act
        var result = _sut.Create(request);

        // Assert
        Assert.Equal("Rex", result.Nome);
        Assert.Equal("Cachorro", result.Especie);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Pet>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void GetById_PetExistente_RetornaPetResponse()
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        // Act
        var result = _sut.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rex", result!.Nome);
    }

    [Fact]
    public void GetById_PetInexistente_RetornaNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        // Act
        var result = _sut.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_RepositorioComPets_RetornaListaDeResponses()
    {
        // Arrange
        var pets = new List<Pet>
        {
            new("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto1.jpg", 1),
            new("Mimi", "Gato", "Persa", 'F', new DateTime(2021, 3, 1), "foto2.jpg", 1)
        };
        _repositoryMock.Setup(r => r.GetAll()).Returns(pets);

        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Update_PetInexistente_RetornaNullENaoChamaSaveChanges()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);
        var request = new PetRequest
        {
            Nome = "Rex",
            Especie = "Cachorro",
            Raca = "Labrador",
            Sexo = 'M',
            DataNascimento = new DateTime(2020, 1, 1),
            FotoUrl = "foto.jpg",
            TutorId = 1
        };

        // Act
        var result = _sut.Update(999, request);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Update_PetExistente_AtualizaERetornaResponseAtualizado()
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);
        var request = new PetRequest
        {
            Nome = "Rex Atualizado",
            Especie = "Cachorro",
            Raca = "Labrador",
            Sexo = 'M',
            DataNascimento = new DateTime(2020, 1, 1),
            FotoUrl = "foto-nova.jpg",
            TutorId = 1
        };

        // Act
        var result = _sut.Update(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rex Atualizado", result!.Nome);
        _repositoryMock.Verify(r => r.Update(pet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_PetExistente_RemoveERetornaTrue()
    {
        // Arrange
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        // Act
        var result = _sut.Delete(1);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(pet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_PetInexistente_RetornaFalseSemChamarRepositorio()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        // Act
        var result = _sut.Delete(999);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Pet>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
