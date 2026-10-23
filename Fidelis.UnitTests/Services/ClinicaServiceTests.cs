using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class ClinicaServiceTests
{
    private readonly Mock<IClinicaRepository> _repositoryMock = new();
    private readonly ClinicaService _sut;

    public ClinicaServiceTests()
    {
        _sut = new ClinicaService(_repositoryMock.Object);
    }

    private static Clinica NovaClinica() => new("Clínica Vida", "12345678000190", "1133334444", "contato@vida.com", "Av. Paulista, 1000");

    [Fact]
    public void Create_RequestValido_PersisteERetornaResponse()
    {
        var request = new ClinicaRequest
        {
            Nome = "Clínica Vida",
            Cnpj = "12345678000190",
            Telefone = "1133334444",
            Email = "contato@vida.com",
            Endereco = "Av. Paulista, 1000"
        };

        var result = _sut.Create(request);

        Assert.Equal("Clínica Vida", result.Nome);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Clinica>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Patch_ApenasTelefoneInformado_AtualizaSomenteTelefone()
    {
        _repositoryMock.Setup(r => r.GetById(1)).Returns(NovaClinica());

        var result = _sut.Patch(1, new ClinicaPatchRequest { Telefone = "1144445555" });

        Assert.Equal("1144445555", result!.Telefone);
        Assert.Equal("Clínica Vida", result.Nome);
    }

    [Fact]
    public void Delete_ClinicaInexistente_RetornaFalse()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Clinica?)null);

        var result = _sut.Delete(10);

        Assert.False(result);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetAll_ComResultados_MapeiaParaResponse()
    {
        var query = new ClinicaQueryParameters { Nome = "vida" };
        _repositoryMock
            .Setup(r => r.GetPaged(query, It.IsAny<IEnumerable<Expression<Func<Clinica, bool>>>>()))
            .Returns(new PagedResult<Clinica>([NovaClinica()], 1, 10, 1));

        var result = _sut.GetAll(query);

        var clinica = Assert.Single(result.Items);
        Assert.Equal("Clínica Vida", clinica.Nome);
    }
}
