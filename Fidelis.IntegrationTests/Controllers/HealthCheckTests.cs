using System.Net;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class HealthCheckTests
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthLive_Sempre_Retorna200()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealthReady_BancoDisponivel_Retorna200()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert
        // No ambiente de testes o DbContext aponta para um banco InMemory
        // sempre acessível, então a verificação de prontidão deve passar.
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
