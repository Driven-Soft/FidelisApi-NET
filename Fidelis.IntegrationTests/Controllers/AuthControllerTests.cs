using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class AuthControllerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static TutorRequest NovoTutorRequest(string cpf, string email) => new()
    {
        Cpf = cpf,
        Nome = "Login Teste",
        Email = email,
        Senha = "123456",
        Telefone = "11988476632",
        Endereco = "Rua de Teste, 1"
    };

    [Fact]
    public async Task PostApiAuthLogin_CredenciaisValidas_Retorna200ComToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest(cpf, email));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "123456" });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task PostApiAuthLogin_SenhaInvalida_Retorna401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest(cpf, email));

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "senha-errada" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostApiAuthLogin_EmailInexistente_Retorna401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = "nao-existe@example.com", Senha = "123456" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetApiPet_SemToken_Retorna401()
    {
        // Arrange
        var client = _factory.CreateClient(); // sem Authorization header

        // Act
        var response = await client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetApiPet_ComTokenValido_Retorna200()
    {
        // Arrange
        var client = await _factory.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
