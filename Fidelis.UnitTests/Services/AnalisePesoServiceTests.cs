using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Application.Settings;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class AnalisePesoServiceTests
{
    private readonly Mock<IPetRepository> _petRepositoryMock = new();
    private readonly Mock<IHistoricoPesoRepository> _historicoPesoRepositoryMock = new();
    private readonly Mock<IRecomendacaoRepository> _recomendacaoRepositoryMock = new();
    private readonly Mock<ILembreteRepository> _lembreteRepositoryMock = new();
    private readonly Mock<IFidelisAiClient> _clientMock = new();
    private readonly FidelisAiSettings _settings = new() { BaseUrl = "http://ia:8000" };
    private readonly AnalisePesoService _sut;

    public AnalisePesoServiceTests()
    {
        _sut = new AnalisePesoService(
            _petRepositoryMock.Object,
            _historicoPesoRepositoryMock.Object,
            _recomendacaoRepositoryMock.Object,
            _lembreteRepositoryMock.Object,
            _clientMock.Object,
            Options.Create(_settings),
            NullLogger<AnalisePesoService>.Instance);
    }

    private static Pet NovoPet(string especie = "Cachorro") =>
        new("Aurora", especie, "Yorkshire Terrier", 'F', new DateTime(2022, 10, 26), "aurora.jpg", 7);

    private static HistoricoPeso NovaPesagem(DateTime? dataMedicao = null) =>
        new(3.2m, dataMedicao ?? new DateTime(2026, 10, 26), 1);

    private static AnalisePesoResponse NovaAnalise(string risco = "AMARELO") => new()
    {
        PetId = 1,
        Avaliacao = new AvaliacaoPeso
        {
            Risco = risco,
            TipoRecomendacao = risco == "AMARELO" ? "PESO_ATENCAO" : "PESO_ADEQUADO",
            FaixaEsperadaKg = new FaixaPeso { Min = 2.1m, Max = 2.7m },
            PesoInformadoKg = 3.2m,
            Justificativa = "3,2 kg está 19% acima da faixa esperada de 2,1 a 2,7 kg.",
            Acoes = risco == "AMARELO"
                ?
                [
                    new AcaoSugerida { Registro = "RECOMENDACAO", Tipo = "PESO_ATENCAO", Descricao = "Vale revisar a alimentação." },
                    new AcaoSugerida { Registro = "LEMBRETE", Tipo = "PESAGEM", Descricao = "Registrar nova pesagem.", PrazoDias = 15 }
                ]
                : []
        }
    };

    private void ConfigurarPetComPesagem(Pet pet, HistoricoPeso pesagem)
    {
        _petRepositoryMock.Setup(r => r.GetById(1)).Returns(pet);
        _historicoPesoRepositoryMock.Setup(r => r.GetLatestByPetId(1)).Returns(pesagem);
    }

    [Fact]
    public void AnalisarPesagem_RiscoAmarelo_GravaRecomendacaoELembreteDePesagem()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise());

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Equal("AMARELO", result!.Risco);
        _recomendacaoRepositoryMock.Verify(r => r.Add(It.Is<Recomendacao>(x => x.Tipo == "PESO_ATENCAO" && x.PetId == 1)), Times.Once);
        _lembreteRepositoryMock.Verify(r => r.Add(It.Is<Lembrete>(x =>
            x.Tipo == "PESAGEM" && x.TutorId == 7 && x.PetId == 1 && x.Status == 'P' &&
            x.DataPrevista == DateTime.UtcNow.Date.AddDays(15))), Times.Once);
        _recomendacaoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void AnalisarPesagem_RiscoVerde_NaoGravaNada()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise("VERDE"));

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Equal("VERDE", result!.Risco);
        _recomendacaoRepositoryMock.Verify(r => r.Add(It.IsAny<Recomendacao>()), Times.Never);
        _lembreteRepositoryMock.Verify(r => r.Add(It.IsAny<Lembrete>()), Times.Never);
    }

    [Fact]
    public void AnalisarPesagem_RecomendacaoDoMesmoTipoNaJanela_NaoRepeteAlerta()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise());
        _recomendacaoRepositoryMock
            .Setup(r => r.ExistsByPetIdAndTipo(1, "PESO_ATENCAO", It.Is<DateTime>(desde => desde <= DateTime.UtcNow.AddDays(-30))))
            .Returns(true);

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Equal("AMARELO", result!.Risco);
        _recomendacaoRepositoryMock.Verify(r => r.Add(It.IsAny<Recomendacao>()), Times.Never);
        _lembreteRepositoryMock.Verify(r => r.Add(It.IsAny<Lembrete>()), Times.Never);
        _recomendacaoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Theory]
    [InlineData("Cão", "Cachorro")]
    [InlineData("  CADELA ", "Cachorro")]
    [InlineData("Canino", "Cachorro")]
    [InlineData("gata", "Gato")]
    [InlineData("Felino", "Gato")]
    public void AnalisarPesagem_EspecieEscritaNoCadastro_EnviaEspecieDoContrato(string especie, string esperada)
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(especie), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise("VERDE"));

        _sut.AnalisarPesagem(pesagem);

        _clientMock.Verify(c => c.Analisar(It.Is<AnalisePesoRequest>(x => x.Especie == esperada)), Times.Once);
    }

    [Fact]
    public void AnalisarPesagem_PerfilDoPet_EnviaIdadeCalculadaNaDataDaMedicao()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise("VERDE"));

        _sut.AnalisarPesagem(pesagem);

        _clientMock.Verify(c => c.Analisar(It.Is<AnalisePesoRequest>(x =>
            x.PetId == 1 && x.Raca == "Yorkshire Terrier" && x.Sexo == "F" && x.IdadeAnos == 4.0 && x.PesoKg == 3.2m)), Times.Once);
    }

    [Fact]
    public void AnalisarPesagem_IaIndisponivel_RetornaNullSemLancar()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Throws(new HttpRequestException("connection refused"));

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Null(result);
        _recomendacaoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void AnalisarPesagem_TempoDeRespostaExcedido_RetornaNullSemLancar()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Throws(new TaskCanceledException("timeout"));

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Null(result);
        _recomendacaoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void AnalisarPesagem_FalhaAoGravarAcoes_RetornaNullSemLancar()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise());
        _recomendacaoRepositoryMock.Setup(r => r.SaveChanges()).Throws(new InvalidOperationException("banco indisponível"));

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Null(result);
    }

    [Fact]
    public void AnalisarPesagem_EspecieNaoAvaliada_NaoChamaAIa()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet("Coelho"), pesagem);

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Null(result);
        _clientMock.Verify(c => c.Analisar(It.IsAny<AnalisePesoRequest>()), Times.Never);
    }

    [Fact]
    public void AnalisarPesagem_PesagemRetroativa_NaoChamaAIa()
    {
        ConfigurarPetComPesagem(NovoPet(), NovaPesagem(new DateTime(2026, 10, 26)));

        var result = _sut.AnalisarPesagem(NovaPesagem(new DateTime(2025, 10, 26)));

        Assert.Null(result);
        _clientMock.Verify(c => c.Analisar(It.IsAny<AnalisePesoRequest>()), Times.Never);
    }

    [Fact]
    public void AnalisarPesagem_SemBaseUrlConfigurada_NaoChamaAIa()
    {
        var pesagem = NovaPesagem();
        ConfigurarPetComPesagem(NovoPet(), pesagem);
        _settings.BaseUrl = string.Empty;

        var result = _sut.AnalisarPesagem(pesagem);

        Assert.Null(result);
        _clientMock.Verify(c => c.Analisar(It.IsAny<AnalisePesoRequest>()), Times.Never);
    }

    [Fact]
    public void GetAvaliacao_PesagemExistente_RetornaAvaliacaoSemGravar()
    {
        ConfigurarPetComPesagem(NovoPet(), NovaPesagem());
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Returns(NovaAnalise());

        var result = _sut.GetAvaliacao(1);

        Assert.Equal(new DateTime(2026, 10, 26), result!.DataMedicao);
        Assert.Equal(2.1m, result.Avaliacao.FaixaEsperadaKg.Min);
        _recomendacaoRepositoryMock.Verify(r => r.Add(It.IsAny<Recomendacao>()), Times.Never);
        _lembreteRepositoryMock.Verify(r => r.Add(It.IsAny<Lembrete>()), Times.Never);
    }

    [Fact]
    public void GetAvaliacao_PetInexistente_RetornaNull()
    {
        _petRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        var result = _sut.GetAvaliacao(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetAvaliacao_PetSemPesagem_RetornaNull()
    {
        _petRepositoryMock.Setup(r => r.GetById(1)).Returns(NovoPet());
        _historicoPesoRepositoryMock.Setup(r => r.GetLatestByPetId(1)).Returns((HistoricoPeso?)null);

        var result = _sut.GetAvaliacao(1);

        Assert.Null(result);
    }

    [Fact]
    public void GetAvaliacao_EspecieNaoAvaliada_LancaInvalidOperationException()
    {
        ConfigurarPetComPesagem(NovoPet("Calopsita"), NovaPesagem());

        Assert.Throws<InvalidOperationException>(() => _sut.GetAvaliacao(1));
        _clientMock.Verify(c => c.Analisar(It.IsAny<AnalisePesoRequest>()), Times.Never);
    }

    [Fact]
    public void GetAvaliacao_IaIndisponivel_PropagaHttpRequestException()
    {
        ConfigurarPetComPesagem(NovoPet(), NovaPesagem());
        _clientMock.Setup(c => c.Analisar(It.IsAny<AnalisePesoRequest>())).Throws(new HttpRequestException("connection refused"));

        Assert.Throws<HttpRequestException>(() => _sut.GetAvaliacao(1));
    }
}
