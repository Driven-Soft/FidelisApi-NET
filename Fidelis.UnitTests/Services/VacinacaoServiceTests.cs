using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class VacinacaoServiceTests
{
    private readonly Mock<IVacinacaoRepository> _repositoryMock = new();
    private readonly VacinacaoService _sut;

    public VacinacaoServiceTests()
    {
        _sut = new VacinacaoService(_repositoryMock.Object);
    }

    [Fact]
    public void Create_DatasInconsistentes_LancaArgumentException()
    {
        var request = new VacinacaoRequest
        {
            VacinaAplicada = "V10",
            DataAplicacao = new DateTime(2026, 5, 1),
            DataProxima = new DateTime(2026, 4, 1),
            PetId = 1,
            VeterinarioId = 1
        };

        Assert.Throws<ArgumentException>(() => _sut.Create(request));
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Patch_SomenteUmaDataInformada_NaoAlteraDatas()
    {
        var aplicacao = new DateTime(2026, 1, 1);
        var proxima = new DateTime(2027, 1, 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(new Vacinacao("V10", aplicacao, proxima, 1, 1));

        var result = _sut.Patch(1, new VacinacaoPatchRequest { DataAplicacao = new DateTime(2026, 2, 1) });

        Assert.Equal(aplicacao, result!.DataAplicacao);
        Assert.Equal(proxima, result.DataProxima);
    }

    [Fact]
    public void Update_DadosValidos_AtualizaVacinaEDatas()
    {
        var vacinacao = new Vacinacao("V8", new DateTime(2026, 1, 1), new DateTime(2027, 1, 1), 1, 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(vacinacao);
        var request = new VacinacaoRequest
        {
            VacinaAplicada = "V10",
            DataAplicacao = new DateTime(2026, 3, 1),
            DataProxima = new DateTime(2027, 3, 1),
            PetId = 1,
            VeterinarioId = 1
        };

        var result = _sut.Update(1, request);

        Assert.Equal("V10", result!.VacinaAplicada);
        Assert.Equal(new DateTime(2027, 3, 1), result.DataProxima);
    }
}
