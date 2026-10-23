using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class TutorControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public TutorControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

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
        var request = NovoTutorRequest("00011122233");

        var response = await _client.PostAsJsonAsync("/api/Tutor", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(request.Nome, body!.Data.Nome);
        Assert.True(body.Data.Id > 0);
    }

    [Fact]
    public async Task GetApiTutorById_TutorExistente_Retorna200ComOTutor()
    {
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("22233344455"));
        var createdTutor = await created.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);

        var response = await _client.GetAsync($"/api/Tutor/{createdTutor!.Data.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);
        Assert.Equal(createdTutor.Data.Id, body!.Data.Id);
    }

    [Fact]
    public async Task GetApiTutorById_TutorInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/Tutor/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutApiTutor_TutorExistente_Retorna200ComDadosAtualizados()
    {
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("33344455566"));
        var createdTutor = await created.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);
        var updateRequest = NovoTutorRequest("33344455566");
        updateRequest.Nome = "Felipe Atualizado";

        var response = await _client.PutAsJsonAsync($"/api/Tutor/{createdTutor!.Data.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);
        Assert.Equal("Felipe Atualizado", body!.Data.Nome);
    }

    [Fact]
    public async Task DeleteApiTutor_TutorExistente_Retorna204EDepoisGetRetorna404()
    {
        var created = await _client.PostAsJsonAsync("/api/Tutor", NovoTutorRequest("44455566677"));
        var createdTutor = await created.Content.ReadFromJsonAsync<ResourceDto<TutorResponseDto>>(JsonOptions);

        var deleteResponse = await _client.DeleteAsync($"/api/Tutor/{createdTutor!.Data.Id}");
        var getResponse = await _client.GetAsync($"/api/Tutor/{createdTutor.Data.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task PostApiTutor_RequestSemNome_Retorna400()
    {
        var request = NovoTutorRequest("55566677788");
        request.Nome = string.Empty;

        var response = await _client.PostAsJsonAsync("/api/Tutor", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
