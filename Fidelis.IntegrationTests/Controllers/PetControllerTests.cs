using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class PetControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public PetControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static PetRequest NovoPetRequest(int tutorId, string nome = "Rex") => new()
    {
        Nome = nome,
        Especie = "Cachorro",
        Raca = "Labrador",
        Sexo = 'M',
        DataNascimento = new DateTime(2020, 1, 1),
        FotoUrl = "https://example.com/rex.jpg",
        TutorId = tutorId
    };

    [Fact]
    public async Task PostApiPet_RequestValido_Retorna201ComPetELinksHateoas()
    {
        var tutorId = _factory.SeedTutor();

        var response = await _client.PostAsJsonAsync("/api/Pet", NovoPetRequest(tutorId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<PetResponseDto>>(JsonOptions);
        Assert.Equal("Rex", body!.Data.Nome);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains(body.Links, l => l.Rel == "self" && l.Method == "GET" && l.Href.EndsWith($"/api/Pet/{body.Data.Id}"));
        Assert.Contains(body.Links, l => l.Rel == "update" && l.Method == "PUT");
        Assert.Contains(body.Links, l => l.Rel == "partial-update" && l.Method == "PATCH");
        Assert.Contains(body.Links, l => l.Rel == "delete" && l.Method == "DELETE");
        Assert.Contains(body.Links, l => l.Rel == "collection");
    }

    [Fact]
    public async Task GetApiPet_AposCriarPet_PaginaContemOPetCriado()
    {
        var tutorId = _factory.SeedTutor();
        await _client.PostAsJsonAsync("/api/Pet", NovoPetRequest(tutorId, "Bidu"));

        var response = await _client.GetAsync($"/api/Pet?tutorId={tutorId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedDto<PetResponseDto>>(JsonOptions);
        var item = Assert.Single(body!.Items);
        Assert.Equal("Bidu", item.Data.Nome);
        Assert.NotEmpty(item.Links);
    }

    [Fact]
    public async Task GetApiPetById_PetExistente_Retorna200ComLinks()
    {
        var petId = _factory.SeedPet("Paçoca");

        var response = await _client.GetAsync($"/api/Pet/{petId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<PetResponseDto>>(JsonOptions);
        Assert.Equal("Paçoca", body!.Data.Nome);
        Assert.Equal(5, body.Links.Count);
    }

    [Fact]
    public async Task GetApiPetById_PetInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/Pet/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchApiPet_ApenasNomeInformado_Retorna200ComNomeAtualizado()
    {
        var tutorId = _factory.SeedTutor();
        var created = await _client.PostAsJsonAsync("/api/Pet", NovoPetRequest(tutorId));
        var createdPet = await created.Content.ReadFromJsonAsync<ResourceDto<PetResponseDto>>(JsonOptions);

        var response = await _client.PatchAsJsonAsync($"/api/Pet/{createdPet!.Data.Id}", new PetPatchRequest { Nome = "Rex Segundo" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<PetResponseDto>>(JsonOptions);
        Assert.Equal("Rex Segundo", body!.Data.Nome);
        Assert.Equal(createdPet.Data.Especie, body.Data.Especie);
    }

    [Fact]
    public async Task DeleteApiPet_PetExistente_Retorna204EDepois404()
    {
        var petId = _factory.SeedPet();

        var deleteResponse = await _client.DeleteAsync($"/api/Pet/{petId}");
        var getResponse = await _client.GetAsync($"/api/Pet/{petId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteApiPet_PetInexistente_Retorna404()
    {
        var response = await _client.DeleteAsync("/api/Pet/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostApiPet_SexoInvalido_Retorna400()
    {
        var tutorId = _factory.SeedTutor();
        var request = NovoPetRequest(tutorId);
        request.Sexo = 'X';

        var response = await _client.PostAsJsonAsync("/api/Pet", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
