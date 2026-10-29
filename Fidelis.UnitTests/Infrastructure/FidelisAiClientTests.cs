using System.Net;
using System.Text;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.Infrastructure.Clients;
using Xunit;

namespace Fidelis.UnitTests.Infrastructure;

public class FidelisAiClientTests
{
    private const string RespostaAurora = """
        {"pet_id": 1,
         "perfil": {"especie": "Cachorro", "raca": "Yorkshire Terrier", "sexo": "F", "idade_anos": 4.0,
                    "raca_na_base_de_referencia": true},
         "avaliacao": {
           "risco": "AMARELO",
           "tipo_recomendacao": "PESO_ATENCAO",
           "faixa_esperada_kg": {"min": 2.1, "max": 2.7},
           "peso_informado_kg": 3.2,
           "desvio_alem_da_faixa": 0.185,
           "justificativa": "3,2 kg está 19% acima da faixa esperada de 2,1 a 2,7 kg para uma Yorkshire Terrier fêmea de 4 anos.",
           "acoes": [
             {"registro": "RECOMENDACAO", "tipo": "PESO_ATENCAO", "descricao": "Vale revisar a alimentação."},
             {"registro": "LEMBRETE", "tipo": "PESAGEM", "prazo_dias": 15, "descricao": "Registrar nova pesagem para acompanhamento."}
           ]}}
        """;

    private static AnalisePesoRequest NovaAnalise() => new()
    {
        PetId = 1,
        Especie = "Cachorro",
        Raca = "Yorkshire Terrier",
        Sexo = "F",
        IdadeAnos = 4.0,
        PesoKg = 3.2m
    };

    private static FidelisAiClient NovoClient(HandlerFalso handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("http://ia:8000/") });

    [Fact]
    public void Analisar_RequestValido_EnviaPostParaAnaliseEmSnakeCase()
    {
        var handler = new HandlerFalso(HttpStatusCode.OK, RespostaAurora);

        NovoClient(handler).Analisar(NovaAnalise());

        Assert.Equal(HttpMethod.Post, handler.Metodo);
        Assert.Equal("http://ia:8000/analise", handler.Url);
        using var json = JsonDocument.Parse(handler.Corpo!);
        Assert.Equal(1, json.RootElement.GetProperty("pet_id").GetInt32());
        Assert.Equal("Cachorro", json.RootElement.GetProperty("especie").GetString());
        Assert.Equal("Yorkshire Terrier", json.RootElement.GetProperty("raca").GetString());
        Assert.Equal("F", json.RootElement.GetProperty("sexo").GetString());
        Assert.Equal(4.0, json.RootElement.GetProperty("idade_anos").GetDouble());
        Assert.Equal(3.2m, json.RootElement.GetProperty("peso_kg").GetDecimal());
    }

    [Fact]
    public void Analisar_RespostaDoServico_DesserializaAvaliacaoEAcoes()
    {
        var handler = new HandlerFalso(HttpStatusCode.OK, RespostaAurora);

        var result = NovoClient(handler).Analisar(NovaAnalise());

        Assert.True(result.Perfil.RacaNaBaseDeReferencia);
        Assert.Equal("AMARELO", result.Avaliacao.Risco);
        Assert.Equal("PESO_ATENCAO", result.Avaliacao.TipoRecomendacao);
        Assert.Equal(2.1m, result.Avaliacao.FaixaEsperadaKg.Min);
        Assert.Equal(2.7m, result.Avaliacao.FaixaEsperadaKg.Max);
        Assert.Equal(0.185, result.Avaliacao.DesvioAlemDaFaixa);
        Assert.Equal(2, result.Avaliacao.Acoes.Count);
        Assert.Null(result.Avaliacao.Acoes[0].PrazoDias);
        Assert.Equal(15, result.Avaliacao.Acoes[1].PrazoDias);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.UnprocessableEntity)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public void Analisar_StatusDeErro_LancaHttpRequestException(HttpStatusCode status)
    {
        var handler = new HandlerFalso(status, "{}");

        Assert.Throws<HttpRequestException>(() => NovoClient(handler).Analisar(NovaAnalise()));
    }

    private sealed class HandlerFalso : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;
        private readonly string _corpo;

        public HandlerFalso(HttpStatusCode status, string corpo)
        {
            _status = status;
            _corpo = corpo;
        }

        public HttpMethod? Metodo { get; private set; }
        public string? Url { get; private set; }
        public string? Corpo { get; private set; }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Metodo = request.Method;
            Url = request.RequestUri!.ToString();
            Corpo = request.Content is null ? null : new StreamReader(request.Content.ReadAsStream(cancellationToken)).ReadToEnd();
            return new HttpResponseMessage(_status) { Content = new StringContent(_corpo, Encoding.UTF8, "application/json") };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(Send(request, cancellationToken));
    }
}
