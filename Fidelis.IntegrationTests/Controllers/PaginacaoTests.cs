using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class PaginacaoTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;
    private int _tutorId;

    public PaginacaoTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
        _tutorId = _factory.SeedTutor();

        foreach (var nome in new[] { "Bolt", "Amendoim", "Cacau", "Duque", "Estrela" })
            _factory.SeedPet(nome, "Cachorro", _tutorId);

        _factory.SeedPet("Frajola", "Gato", _tutorId);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetApiPet_PrimeiraPagina_RetornaMetadadosELinksDeNavegacao()
    {
        var response = await _client.GetAsync($"/api/Pet?tutorId={_tutorId}&page=1&pageSize=2&sortBy=nome");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedDto<PetResponseDto>>(JsonOptions);
        Assert.Equal(6, body!.TotalItems);
        Assert.Equal(3, body.TotalPages);
        Assert.Equal(2, body.Items.Count);
        Assert.Equal("Amendoim", body.Items[0].Data.Nome);
        Assert.Contains(body.Links, l => l.Rel == "next" && l.Href.Contains("page=2") && l.Href.Contains($"tutorId={_tutorId}"));
        Assert.Contains(body.Links, l => l.Rel == "last" && l.Href.Contains("page=3"));
        Assert.DoesNotContain(body.Links, l => l.Rel == "previous");
        Assert.Contains(body.Links, l => l.Rel == "create" && l.Method == "POST");
    }

    [Fact]
    public async Task GetApiPet_UltimaPagina_PossuiPreviousESemNext()
    {
        var response = await _client.GetAsync($"/api/Pet?tutorId={_tutorId}&page=3&pageSize=2&sortBy=nome");

        var body = await response.Content.ReadFromJsonAsync<PagedDto<PetResponseDto>>(JsonOptions);
        Assert.Contains(body!.Links, l => l.Rel == "previous" && l.Href.Contains("page=2"));
        Assert.DoesNotContain(body.Links, l => l.Rel == "next");
    }

    [Fact]
    public async Task GetApiPet_OrdenacaoDescendente_RetornaOrdemInversa()
    {
        var response = await _client.GetAsync($"/api/Pet?tutorId={_tutorId}&sortBy=nome&sortDirection=desc&pageSize=3");

        var body = await response.Content.ReadFromJsonAsync<PagedDto<PetResponseDto>>(JsonOptions);
        Assert.Equal(new List<string> { "Frajola", "Estrela", "Duque" }, body!.Items.Select(i => i.Data.Nome).ToList());
    }

    [Fact]
    public async Task GetApiPet_FiltroPorEspecie_RetornaSomenteGatos()
    {
        var response = await _client.GetAsync($"/api/Pet?tutorId={_tutorId}&especie=gato");

        var body = await response.Content.ReadFromJsonAsync<PagedDto<PetResponseDto>>(JsonOptions);
        var item = Assert.Single(body!.Items);
        Assert.Equal("Frajola", item.Data.Nome);
    }

    [Fact]
    public async Task GetApiPet_CampoDeOrdenacaoInvalido_Retorna400()
    {
        var response = await _client.GetAsync("/api/Pet?sortBy=campoQueNaoExiste");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("pageSize=0")]
    [InlineData("pageSize=101")]
    [InlineData("page=0")]
    [InlineData("sortDirection=aleatorio")]
    public async Task GetApiPet_ParametrosDePaginacaoInvalidos_Retorna400(string queryString)
    {
        var response = await _client.GetAsync($"/api/Pet?{queryString}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
