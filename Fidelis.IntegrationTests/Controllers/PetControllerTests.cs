using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;
using Fidelis.IntegrationTests.Dtos;
using Xunit;

namespace Fidelis.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class PetControllerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PetControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Cria um Tutor diretamente no banco InMemory (via DbContext) para servir
    /// de dono do Pet nos testes, evitando repetir o fluxo de criação de Tutor
    /// via HTTP em todo teste.
    /// </summary>
    private int CriarTutorDeApoio()
    {
        using var context = _factory.CreateDbContext();
        var tutor = new Tutor(
            cpf: Guid.NewGuid().ToString("N")[..11],
            nome: "Tutor de Apoio",
            email: $"{Guid.NewGuid():N}@example.com",
            senha: "123456",
            telefone: "11988476632",
            endereco: "Rua de Apoio, 1");

        context.Tutores.Add(tutor);
        context.SaveChanges();
        return tutor.Id;
    }

    private static PetRequest NovoPetRequest(int tutorId) => new()
    {
        Nome = "Rex",
        Especie = "Cachorro",
        Raca = "Labrador",
        Sexo = 'M',
        DataNascimento = new DateTime(2020, 1, 1),
        FotoUrl = "https://example.com/rex.jpg",
        TutorId = tutorId
    };

    [Fact]
    public async Task PostApiPet_RequestValido_Retorna201ComPetCriado()
    {
        // Arrange
        var tutorId = CriarTutorDeApoio();
        var request = NovoPetRequest(tutorId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/Pet", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PetResponseDto>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Rex", body!.Nome);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task GetApiPet_ApósCriarPet_ListaContemOPetCriado()
    {
        // Arrange
        var tutorId = CriarTutorDeApoio();
        await _client.PostAsJsonAsync("/api/Pet", NovoPetRequest(tutorId));

        // Act
        var response = await _client.GetAsync("/api/Pet");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<List<PetResponseDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Contains(body!, p => p.Nome == "Rex");
    }

    [Fact]
    public async Task GetApiPetById_PetInexistente_Retorna404()
    {
        // Act
        var response = await _client.GetAsync("/api/Pet/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchApiPet_ApenasNomeInformado_Retorna200ComNomeAtualizado()
    {
        // Arrange
        var tutorId = CriarTutorDeApoio();
        var created = await _client.PostAsJsonAsync("/api/Pet", NovoPetRequest(tutorId));
        var createdPet = await created.Content.ReadFromJsonAsync<PetResponseDto>(JsonOptions);
        var patchRequest = new PetPatchRequest { Nome = "Rex Segundo" };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/Pet/{createdPet!.Id}", patchRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PetResponseDto>(JsonOptions);
        Assert.Equal("Rex Segundo", body!.Nome);
        Assert.Equal(createdPet.Especie, body.Especie);
    }

    [Fact]
    public async Task DeleteApiPet_PetInexistente_Retorna404()
    {
        // Act
        var response = await _client.DeleteAsync("/api/Pet/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostApiPet_SexoInvalido_Retorna400()
    {
        // Arrange
        var tutorId = CriarTutorDeApoio();
        var request = NovoPetRequest(tutorId);
        request.Sexo = 'X'; // regra de domínio só aceita 'M' ou 'F'

        // Act
        var response = await _client.PostAsJsonAsync("/api/Pet", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
