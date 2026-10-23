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
        var response = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealthReady_BancoDisponivel_Retorna200()
    {
        var response = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_RelatorioCompleto_RetornaJsonComStatus()
    {
        var response = await _client.GetAsync("/health");
        var conteudo = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"status\"", conteudo);
        Assert.Contains("oracle-database", conteudo);
    }
}
