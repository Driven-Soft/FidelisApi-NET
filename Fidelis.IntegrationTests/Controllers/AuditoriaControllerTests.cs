using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class AuditoriaControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public AuditoriaControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<PagedDto<AuditoriaResponseDto>> AguardarAuditoriaAsync(string caminho, int statusCode)
    {
        PagedDto<AuditoriaResponseDto>? body = null;

        for (var tentativa = 0; tentativa < 50; tentativa++)
        {
            var response = await _client.GetAsync($"/api/Auditoria?caminho={Uri.EscapeDataString(caminho)}&statusCode={statusCode}");
            body = await response.Content.ReadFromJsonAsync<PagedDto<AuditoriaResponseDto>>(JsonOptions);

            if (body!.TotalItems > 0)
                return body;

            await Task.Delay(100);
        }

        return body!;
    }

    [Fact]
    public async Task RequisicaoDeEscrita_GeraLogDeAuditoriaComUsuario()
    {
        var tutorId = _factory.SeedTutor();
        var petResponse = await _client.PostAsJsonAsync("/api/Pet", new PetRequest
        {
            Nome = "Auditado",
            Especie = "Gato",
            Raca = "SRD",
            Sexo = 'F',
            DataNascimento = new DateTime(2022, 1, 1),
            FotoUrl = "https://example.com/gato.jpg",
            TutorId = tutorId
        });
        var pet = await petResponse.Content.ReadFromJsonAsync<ResourceDto<PetResponseDto>>(JsonOptions);

        var auditoria = await AguardarAuditoriaAsync("/api/Pet", 201);

        Assert.NotEmpty(auditoria.Items);
        var log = auditoria.Items.First().Data;
        Assert.Equal("POST", log.Metodo);
        Assert.True(log.Sucesso);
        Assert.False(string.IsNullOrWhiteSpace(log.UsuarioId));
        Assert.NotNull(pet);
    }

    [Fact]
    public async Task RequisicaoComErro_RegistraStatusDeFalha()
    {
        await _client.DeleteAsync("/api/Clinica/999999");

        var auditoria = await AguardarAuditoriaAsync("/api/Clinica/999999", 404);

        var log = Assert.Single(auditoria.Items).Data;
        Assert.Equal("DELETE", log.Metodo);
        Assert.False(log.Sucesso);
    }

    [Fact]
    public async Task GetById_LogInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/Auditoria/0123456789abcdef01234567");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
