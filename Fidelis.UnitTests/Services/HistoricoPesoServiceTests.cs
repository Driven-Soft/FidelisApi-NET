using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Application.Services.Interfaces;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class HistoricoPesoServiceTests
{
    private readonly Mock<IHistoricoPesoRepository> _repositoryMock = new();
    private readonly Mock<IAnalisePesoService> _analisePesoServiceMock = new();
    private readonly HistoricoPesoService _sut;

    public HistoricoPesoServiceTests()
    {
        _sut = new HistoricoPesoService(_repositoryMock.Object, _analisePesoServiceMock.Object);
    }

    private static HistoricoPesoRequest NovoRequest() => new() { PesoKg = 3.2m, DataMedicao = new DateTime(2026, 10, 26), PetId = 1 };

    [Fact]
    public void Patch_SemPeso_MantemPesoOriginal()
    {
        _repositoryMock.Setup(r => r.GetById(1)).Returns(new HistoricoPeso(10m, new DateTime(2026, 1, 1), 1));

        var result = _sut.Patch(1, new HistoricoPesoPatchRequest());

        Assert.Equal(10m, result!.PesoKg);
    }

    [Fact]
    public void Create_PesoNegativo_LancaArgumentException()
    {
        var request = new HistoricoPesoRequest { PesoKg = -1m, DataMedicao = DateTime.UtcNow, PetId = 1 };

        Assert.Throws<ArgumentException>(() => _sut.Create(request));
    }

    [Fact]
    public void Create_RequestValido_GravaAPesagemAntesDaAnalise()
    {
        var etapas = new List<string>();
        _repositoryMock.Setup(r => r.SaveChanges()).Callback(() => etapas.Add("gravou"));
        _analisePesoServiceMock.Setup(a => a.AnalisarPesagem(It.IsAny<HistoricoPeso>())).Callback(() => etapas.Add("analisou"));

        _sut.Create(NovoRequest());

        Assert.Equal(new List<string> { "gravou", "analisou" }, etapas);
    }

    [Fact]
    public void Create_IaAvaliouAPesagem_DevolveAvaliacaoNaResposta()
    {
        var avaliacao = new AvaliacaoPeso { Risco = "AMARELO", TipoRecomendacao = "PESO_ATENCAO" };
        _analisePesoServiceMock.Setup(a => a.AnalisarPesagem(It.IsAny<HistoricoPeso>())).Returns(avaliacao);

        var result = _sut.Create(NovoRequest());

        Assert.Equal(3.2m, result.PesoKg);
        Assert.Same(avaliacao, result.AvaliacaoIa);
    }

    [Fact]
    public void Create_IaIndisponivel_GravaPesagemSemAvaliacao()
    {
        _analisePesoServiceMock.Setup(a => a.AnalisarPesagem(It.IsAny<HistoricoPeso>())).Returns((AvaliacaoPeso?)null);

        var result = _sut.Create(NovoRequest());

        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        Assert.Null(result.AvaliacaoIa);
    }
}
