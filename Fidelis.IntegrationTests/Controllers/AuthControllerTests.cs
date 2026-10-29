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
        var client = _factory.CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest(cpf, email));

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "123456" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
    }

    [Fact]
    public async Task PostApiAuthLogin_SenhaInvalida_Retorna401()
    {
        var client = _factory.CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest(cpf, email));

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "senha-errada" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostApiAuthLogin_EmailInexistente_Retorna401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = "nao-existe@example.com", Senha = "123456" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostApiAuthLogin_TutorComTipo_RetornaSessaoNoFormatoDoApp()
    {
        var client = _factory.CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        await client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest(cpf, email));

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "123456", Tipo = "TUTOR" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
        Assert.Equal("TUTOR", body.GetProperty("tipo").GetString());
        Assert.True(body.GetProperty("tutorId").GetInt32() > 0);
        Assert.Equal(JsonValueKind.Null, body.GetProperty("veterinarioId").ValueKind);
        Assert.Equal(email, body.GetProperty("email").GetString());
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?Z$", body.GetProperty("expiraEm").GetString());
    }

    [Fact]
    public async Task PostApiAuthLogin_Veterinario_Retorna200ComVeterinarioId()
    {
        var email = $"{Guid.NewGuid():N}@example.com";
        var veterinarioId = _factory.SeedVeterinario(email);
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = email, Senha = "123456", Tipo = "VETERINARIO" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.Equal("VETERINARIO", body!.Tipo);
        Assert.Equal(veterinarioId, body.VeterinarioId);
        Assert.Null(body.TutorId);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task PostApiAuthLogin_TipoInvalido_Retorna400()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/login",
            new LoginRequest { Email = "alguem@example.com", Senha = "123456", Tipo = "ADMIN" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetApiPet_SemToken_Retorna401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/Pet");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetApiPet_ComTokenValido_Retorna200()
    {
        var client = await _factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/Pet");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
