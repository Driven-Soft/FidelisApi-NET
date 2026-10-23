using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class LogAuditoriaTests
{
    [Theory]
    [InlineData(200, true)]
    [InlineData(201, true)]
    [InlineData(400, false)]
    [InlineData(500, false)]
    public void Sucesso_DeAcordoComStatusCode_RetornaValorEsperado(int statusCode, bool esperado)
    {
        var log = new LogAuditoria("post", "/api/Pet", statusCode, "1", "a@a.com", 15);

        Assert.Equal(esperado, log.Sucesso);
        Assert.Equal("POST", log.Metodo);
    }

    [Theory]
    [InlineData("", "/api/Pet", 200)]
    [InlineData("POST", "", 200)]
    [InlineData("POST", "/api/Pet", 99)]
    [InlineData("POST", "/api/Pet", 600)]
    public void Constructor_DadosInvalidos_LancaArgumentException(string metodo, string caminho, int statusCode)
    {
        var act = () => new LogAuditoria(metodo, caminho, statusCode, null, null, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_DuracaoNegativa_LancaArgumentException()
    {
        var act = () => new LogAuditoria("GET", "/api/Pet", 200, null, null, -1);

        Assert.Throws<ArgumentException>(act);
    }
}
