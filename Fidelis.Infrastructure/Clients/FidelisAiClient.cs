using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;

namespace Fidelis.Infrastructure.Clients;

public class FidelisAiClient : IFidelisAiClient
{
    public const string ApiKeyHeader = "X-API-Key";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;

    public FidelisAiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public AnalisePesoResponse Analisar(AnalisePesoRequest request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "analise")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };

        using var response = _httpClient.Send(message);
        response.EnsureSuccessStatusCode();

        using var body = response.Content.ReadAsStream();
        return JsonSerializer.Deserialize<AnalisePesoResponse>(body, JsonOptions)
            ?? throw new InvalidOperationException("O serviço de IA devolveu uma resposta vazia.");
    }
}
