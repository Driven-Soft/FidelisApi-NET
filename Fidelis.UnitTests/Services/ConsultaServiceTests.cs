using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _repositoryMock;
    private readonly ConsultaService _sut;

    public ConsultaServiceTests()
    {
        _repositoryMock = new Mock<IConsultaRepository>();
        _sut = new ConsultaService(_repositoryMock.Object, NullLogger<ConsultaService>.Instance);
    }

    [Fact]
    public void Create_RequestValido_AdicionaConsultaERetornaResponse()
    {
        // Arrange
        var request = new ConsultaRequest
        {
            DataHora = new DateTime(2026, 1, 10, 9, 0, 0),
            Tipo = "Rotina",
            VeterinarioId = 1,
            PetId = 1
        };

        // Act
        var result = _sut.Create(request);

        // Assert
        Assert.Equal("Rotina", result.Tipo);
        Assert.Equal(1, result.PetId);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Consulta>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Patch_ApenasDiagnosticoInformado_AtualizaSomenteEsseCampo()
    {
        // Arrange
        var consulta = new Consulta(new DateTime(2026, 1, 10), "Rotina", 1, 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(consulta);
        var request = new ConsultaPatchRequest { Diagnostico = "Saudável" };

        // Act
        var result = _sut.Patch(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Saudável", result!.Diagnostico);
        Assert.Equal("Rotina", result.Tipo);
    }

    [Fact]
    public void Update_ConsultaInexistente_RetornaNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Consulta?)null);
        var request = new ConsultaRequest
        {
            DataHora = DateTime.UtcNow,
            Tipo = "Retorno",
            VeterinarioId = 1,
            PetId = 1
        };

        // Act
        var result = _sut.Update(999, request);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetAll_RepositorioVazio_RetornaListaVazia()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAll()).Returns(new List<Consulta>());

        // Act
        var result = _sut.GetAll();

        // Assert
        Assert.Empty(result);
    }
}
