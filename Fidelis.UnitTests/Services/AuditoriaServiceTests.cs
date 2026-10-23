using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class AuditoriaServiceTests
{
    private readonly Mock<IAuditoriaRepository> _repositoryMock = new();
    private readonly AuditoriaService _sut;

    public AuditoriaServiceTests()
    {
        _sut = new AuditoriaService(_repositoryMock.Object);
    }

    [Fact]
    public void Registrar_LogValido_GravaNoRepositorio()
    {
        var log = new LogAuditoria("POST", "/api/Pet", 201, "1", "a@a.com", 12);

        _sut.Registrar(log);

        _repositoryMock.Verify(r => r.Add(log), Times.Once);
    }

    [Fact]
    public void Registrar_LogNulo_LancaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _sut.Registrar(null!));
    }

    [Fact]
    public void GetAll_MapeiaLogsParaResponse()
    {
        var query = new AuditoriaQueryParameters { Metodo = "POST" };
        var logs = new List<LogAuditoria> { new("POST", "/api/Pet", 400, null, null, 3) };
        _repositoryMock.Setup(r => r.GetPaged(query)).Returns(new PagedResult<LogAuditoria>(logs, 1, 10, 1));

        var result = _sut.GetAll(query);

        var item = Assert.Single(result.Items);
        Assert.False(item.Sucesso);
        Assert.Equal(400, item.StatusCode);
    }

    [Fact]
    public void GetById_Inexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<string>())).Returns((LogAuditoria?)null);

        Assert.Null(_sut.GetById("x"));
    }
}
