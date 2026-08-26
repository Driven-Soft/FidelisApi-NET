using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _repositoryMock;
    private readonly TutorService _sut;

    public TutorServiceTests()
    {
        _repositoryMock = new Mock<ITutorRepository>();
        _sut = new TutorService(_repositoryMock.Object, NullLogger<TutorService>.Instance);
    }

    [Fact]
    public void Create_RequestValido_AdicionaTutorERetornaResponse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Tutor>());

        var request = new TutorRequest
        {
            Cpf = "12345678900",
            Nome = "Felipe",
            Email = "felipe@example.com",
            Senha = "123456",
            Telefone = "11988476632",
            Endereco = "Rua Exemplo, 123"
        };

        // Act
        var result = _sut.Create(request);

        // Assert
        Assert.Equal("Felipe", result.Nome);
        Assert.Equal("felipe@example.com", result.Email);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Create_EmailJaCadastrado_LancaInvalidOperationExceptionENaoAdiciona()
    {
        // Arrange
        var tutorExistente = new Tutor("11122233344", "Outro Tutor", "felipe@example.com", "123456", "11988476632", "Rua Existente, 1");
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Tutor> { tutorExistente });

        var request = new TutorRequest
        {
            Cpf = "12345678900",
            Nome = "Felipe",
            Email = "felipe@example.com",
            Senha = "123456",
            Telefone = "11988476632",
            Endereco = "Rua Exemplo, 123"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.Create(request));
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetById_TutorInexistente_RetornaNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Tutor?)null);

        // Act
        var result = _sut.GetById(42);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Patch_ApenasNomeInformado_MantemDemaisCamposOriginais()
    {
        // Arrange
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");
        _repositoryMock.Setup(r => r.GetById(1)).Returns(tutor);
        var request = new TutorPatchRequest { Nome = "Felipe Bezerra" };

        // Act
        var result = _sut.Patch(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Felipe Bezerra", result!.Nome);
        Assert.Equal("felipe@example.com", result.Email);
        Assert.Equal("12345678900", result.Cpf);
    }

    [Fact]
    public void Delete_TutorExistente_RemoveERetornaTrue()
    {
        // Arrange
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");
        _repositoryMock.Setup(r => r.GetById(1)).Returns(tutor);

        // Act
        var result = _sut.Delete(1);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(tutor), Times.Once);
    }

    [Fact]
    public void Delete_TutorInexistente_RetornaFalse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Tutor?)null);

        // Act
        var result = _sut.Delete(999);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
