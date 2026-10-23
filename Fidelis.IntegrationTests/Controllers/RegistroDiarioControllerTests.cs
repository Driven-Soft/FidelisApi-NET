using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class RegistroDiarioControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;
    private int _petId;

    public RegistroDiarioControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
        _petId = _factory.SeedPet("Pipoca");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private RegistroDiarioRequest NovoRequest(string titulo = "Passeio no parque", string humor = "Feliz") => new()
    {
        PetId = _petId,
        Titulo = titulo,
        Descricao = "Correu bastante e brincou com outros cães.",
        Humor = humor,
        Tags = ["passeio", "Energia"],
        DataRegistro = new DateTime(2026, 10, 1, 10, 0, 0, DateTimeKind.Utc)
    };

    private async Task<ResourceDto<RegistroDiarioResponseDto>> CriarRegistroAsync(string titulo = "Passeio no parque")
    {
        var response = await _client.PostAsJsonAsync("/api/RegistroDiario", NovoRequest(titulo));
        return (await response.Content.ReadFromJsonAsync<ResourceDto<RegistroDiarioResponseDto>>(JsonOptions))!;
    }

    [Fact]
    public async Task Post_PetExistente_Retorna201ComIdObjectIdELinks()
    {
        var response = await _client.PostAsJsonAsync("/api/RegistroDiario", NovoRequest());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<RegistroDiarioResponseDto>>(JsonOptions);
        Assert.Equal(24, body!.Data.Id.Length);
        Assert.Equal(new List<string> { "passeio", "energia" }, body.Data.Tags);
        Assert.Contains(body.Links, l => l.Rel == "self" && l.Href.EndsWith($"/api/RegistroDiario/{body.Data.Id}"));
        Assert.Contains(body.Links, l => l.Rel == "delete");
    }

    [Fact]
    public async Task Post_PetInexistente_Retorna400()
    {
        var request = NovoRequest();
        request.PetId = 987654;

        var response = await _client.PostAsJsonAsync("/api/RegistroDiario", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_HumorInvalido_Retorna400()
    {
        var response = await _client.PostAsJsonAsync("/api/RegistroDiario", NovoRequest(humor: "Radiante"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_RegistroExistente_Retorna200()
    {
        var criado = await CriarRegistroAsync();

        var response = await _client.GetAsync($"/api/RegistroDiario/{criado.Data.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<RegistroDiarioResponseDto>>(JsonOptions);
        Assert.Equal(criado.Data.Id, body!.Data.Id);
    }

    [Fact]
    public async Task GetById_IdInexistente_Retorna404()
    {
        var response = await _client.GetAsync("/api/RegistroDiario/0123456789abcdef01234567");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_FiltroPorPetETag_RetornaSomenteRegistrosDoPet()
    {
        await CriarRegistroAsync("Primeiro");
        await CriarRegistroAsync("Segundo");
        var outroPet = _factory.SeedPet("Outro");
        var outro = NovoRequest();
        outro.PetId = outroPet;
        await _client.PostAsJsonAsync("/api/RegistroDiario", outro);

        var response = await _client.GetAsync($"/api/RegistroDiario?petId={_petId}&tag=passeio&pageSize=1");

        var body = await response.Content.ReadFromJsonAsync<PagedDto<RegistroDiarioResponseDto>>(JsonOptions);
        Assert.True(body!.TotalItems >= 2);
        Assert.Single(body.Items);
        Assert.All(body.Items, i => Assert.Equal(_petId, i.Data.PetId));
        Assert.Contains(body.Links, l => l.Rel == "next");
    }

    [Fact]
    public async Task Patch_ApenasHumor_Retorna200ComHumorAtualizado()
    {
        var criado = await CriarRegistroAsync();

        var response = await _client.PatchAsJsonAsync($"/api/RegistroDiario/{criado.Data.Id}", new RegistroDiarioPatchRequest { Humor = "calmo" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<RegistroDiarioResponseDto>>(JsonOptions);
        Assert.Equal("Calmo", body!.Data.Humor);
        Assert.Equal(criado.Data.Titulo, body.Data.Titulo);
        Assert.NotNull(body.Data.AtualizadoEm);
    }

    [Fact]
    public async Task Put_RegistroExistente_SubstituiDados()
    {
        var criado = await CriarRegistroAsync();
        var request = new RegistroDiarioUpdateRequest
        {
            Titulo = "Consulta",
            Descricao = "Foi ao veterinário",
            Humor = "Ansioso",
            Tags = ["veterinario"],
            DataRegistro = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc)
        };

        var response = await _client.PutAsJsonAsync($"/api/RegistroDiario/{criado.Data.Id}", request);

        var body = await response.Content.ReadFromJsonAsync<ResourceDto<RegistroDiarioResponseDto>>(JsonOptions);
        Assert.Equal("Consulta", body!.Data.Titulo);
        Assert.Equal(new List<string> { "veterinario" }, body.Data.Tags);
    }

    [Fact]
    public async Task Delete_RegistroExistente_Retorna204EDepois404()
    {
        var criado = await CriarRegistroAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/RegistroDiario/{criado.Data.Id}");
        var getResponse = await _client.GetAsync($"/api/RegistroDiario/{criado.Data.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAll_SemToken_Retorna401()
    {
        var anonimo = _factory.CreateClient();

        var response = await anonimo.GetAsync("/api/RegistroDiario");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
