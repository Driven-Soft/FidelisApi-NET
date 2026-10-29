using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;
using Fidelis.IntegrationTests.Dtos;
using Fidelis.IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class HistoricoPesoControllerTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public HistoricoPesoControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private InMemoryFidelisAiClient FidelisAi => (InMemoryFidelisAiClient)_factory.Services.GetRequiredService<IFidelisAiClient>();

    public async Task InitializeAsync()
    {
        _client = await _factory.CreateAuthenticatedClientAsync();
        FidelisAi.ResponderCom("VERDE");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private Task<HttpResponseMessage> RegistrarPesagemAsync(int petId, decimal pesoKg, DateTime? dataMedicao = null) =>
        _client.PostAsJsonAsync("/api/HistoricoPeso", new HistoricoPesoRequest
        {
            PesoKg = pesoKg,
            DataMedicao = dataMedicao ?? DateTime.UtcNow.Date,
            PetId = petId
        });

    [Fact]
    public async Task Post_RiscoAmarelo_GravaRecomendacaoELembreteDoTutor()
    {
        var tutorId = _factory.SeedTutor();
        var petId = _factory.SeedPet("Aurora", tutorId: tutorId);
        FidelisAi.ResponderCom("AMARELO");

        var response = await RegistrarPesagemAsync(petId, 3.2m);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<HistoricoPesoResponseDto>>(JsonOptions);
        Assert.Equal(3.2m, body!.Data.PesoKg);
        Assert.Equal("AMARELO", body.Data.AvaliacaoIa!.Risco);
        using var context = _factory.CreateDbContext();
        var recomendacao = Assert.Single(context.Recomendacoes.Where(r => r.PetId == petId));
        Assert.Equal("PESO_ATENCAO", recomendacao.Tipo);
        var lembrete = Assert.Single(context.Lembretes.Where(l => l.PetId == petId));
        Assert.Equal("PESAGEM", lembrete.Tipo);
        Assert.Equal(tutorId, lembrete.TutorId);
        Assert.Equal(DateTime.UtcNow.Date.AddDays(15), lembrete.DataPrevista);
    }

    [Fact]
    public async Task Post_IaIndisponivel_GravaPesagemSemAvaliacao()
    {
        var petId = _factory.SeedPet("Thor");
        FidelisAi.Falhar();

        var response = await RegistrarPesagemAsync(petId, 30m);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<HistoricoPesoResponseDto>>(JsonOptions);
        Assert.Null(body!.Data.AvaliacaoIa);
        using var context = _factory.CreateDbContext();
        Assert.Single(context.HistoricoPesos.Where(h => h.PetId == petId));
        Assert.Empty(context.Recomendacoes.Where(r => r.PetId == petId));
    }

    [Fact]
    public async Task Post_MesmoAlertaEmSequencia_NaoDuplicaRecomendacao()
    {
        var petId = _factory.SeedPet("Mel");
        FidelisAi.ResponderCom("AMARELO");

        await RegistrarPesagemAsync(petId, 3.2m, DateTime.UtcNow.Date.AddDays(-1));
        await RegistrarPesagemAsync(petId, 3.3m);

        using var context = _factory.CreateDbContext();
        Assert.Single(context.Recomendacoes.Where(r => r.PetId == petId));
        Assert.Single(context.Lembretes.Where(l => l.PetId == petId));
    }

    [Fact]
    public async Task Post_RiscoEscalaParaVermelho_GravaNovoAlerta()
    {
        var petId = _factory.SeedPet("Nina");
        FidelisAi.ResponderCom("AMARELO");
        await RegistrarPesagemAsync(petId, 3.2m, DateTime.UtcNow.Date.AddDays(-1));
        FidelisAi.ResponderCom("VERMELHO");

        await RegistrarPesagemAsync(petId, 5.0m);

        using var context = _factory.CreateDbContext();
        var tipos = context.Recomendacoes.Where(r => r.PetId == petId).Select(r => r.Tipo).OrderBy(t => t).ToList();
        Assert.Equal(new List<string> { "PESO_ALERTA", "PESO_ATENCAO" }, tipos);
    }

    [Fact]
    public async Task GetAvaliacao_PetComPesagens_AvaliaAMaisRecenteSemGravar()
    {
        var petId = _factory.SeedPet("Aurora");
        await RegistrarPesagemAsync(petId, 2.5m, DateTime.UtcNow.Date.AddDays(-30));
        await RegistrarPesagemAsync(petId, 2.6m);
        FidelisAi.ResponderCom("AMARELO");

        var response = await _client.GetAsync($"/api/HistoricoPeso/pet/{petId}/avaliacao");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ResourceDto<AvaliacaoPesoResponseDto>>(JsonOptions);
        Assert.Equal(petId, body!.Data.PetId);
        Assert.Equal(DateTime.UtcNow.Date, body.Data.DataMedicao);
        Assert.Equal(2.6m, body.Data.Avaliacao.PesoInformadoKg);
        Assert.Equal(2.1m, body.Data.Avaliacao.FaixaEsperadaKg.Min);
        Assert.Contains(body.Links, l => l.Rel == "self" && l.Href.EndsWith($"/api/HistoricoPeso/{body.Data.HistoricoPesoId}"));
        using var context = _factory.CreateDbContext();
        Assert.Empty(context.Recomendacoes.Where(r => r.PetId == petId));
    }

    [Fact]
    public async Task GetAvaliacao_PetSemPesagens_Retorna404()
    {
        var petId = _factory.SeedPet("Pipoca");

        var response = await _client.GetAsync($"/api/HistoricoPeso/pet/{petId}/avaliacao");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAvaliacao_EspecieNaoAvaliada_Retorna400()
    {
        var petId = _factory.SeedPet("Pernalonga", especie: "Coelho");
        await RegistrarPesagemAsync(petId, 1.8m);

        var response = await _client.GetAsync($"/api/HistoricoPeso/pet/{petId}/avaliacao");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAvaliacao_IaIndisponivel_Retorna503()
    {
        var petId = _factory.SeedPet("Bolt");
        await RegistrarPesagemAsync(petId, 30m);
        FidelisAi.Falhar();

        var response = await _client.GetAsync($"/api/HistoricoPeso/pet/{petId}/avaliacao");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
