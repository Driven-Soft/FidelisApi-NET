using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class TutorControllerTests
{
    // A API serializa a saída em camelCase (JsonSerializerDefaults.Web); o
    // HttpClient por padrão desserializa com correspondência case-sensitive,
    // então precisamos explicitar essas opções ao ler as respostas.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client;

    public TutorControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static TutorRequest NovoTutorRequest(string cpf) => new()
    {
        Cpf = cpf,
        Nome = "Felipe Bezerra",
        Email = $"tutor{cpf}@example.com",
        Senha = "123456",
        Telefone = "11988476632",
        Endereco = "Rua Jose das Couves, 100"
    };

    [Fact]
    public async Task PostApiTutor_RequestValido_Retorna201ComTutorCriado()
    {
        // Arrange
        var request = NovoTutorRequest("00011122233");

        // Act
        var response = await _client.PostAsJsonAsync("/api/Tutor", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(request.Nome, body!.Nome);
        Assert.True(body.Id > 0);
    }

    [Fact]
    public async Task GetApiTutorById_TutorExistente_Retorna200ComOTutor()
    {
        // Arrange
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("22233344455"));
        var createdTutor = await created.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);

        // Act
        var response = await _client.GetAsync($"/api/Tutor/{createdTutor!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);
        Assert.Equal(createdTutor.Id, body!.Id);
    }

    [Fact]
    public async Task GetApiTutorById_TutorInexistente_Retorna404()
    {
        // Act
        var response = await _client.GetAsync("/api/Tutor/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutApiTutor_TutorExistente_Retorna200ComDadosAtualizados()
    {
        // Arrange
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("33344455566"));
        var createdTutor = await created.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);
        var updateRequest = NovoTutorRequest("33344455566");
        updateRequest.Nome = "Felipe Atualizado";

        // Act
        var response = await _client.PutAsJsonAsync($"/api/Tutor/{createdTutor!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);
        Assert.Equal("Felipe Atualizado", body!.Nome);
    }

    [Fact]
    public async Task DeleteApiTutor_TutorExistente_Retorna204EDepoisGetRetorna404()
    {
        // Arrange
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("44455566677"));
        var createdTutor = await created.Content.ReadFromJsonAsync<TutorResponseDto>(JsonOptions);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/Tutor/{createdTutor!.Id}");
        var getResponse = await _client.GetAsync($"/api/Tutor/{createdTutor.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task PostApiTutor_RequestSemNome_Retorna400()
    {
        // Arrange
        var request = NovoTutorRequest("55566677788");
        request.Nome = string.Empty;

        // Act
        var response = await _client.PostAsJsonAsync("/api/Tutor", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
