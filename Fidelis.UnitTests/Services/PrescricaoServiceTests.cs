using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class PrescricaoServiceTests
{
    private readonly Mock<IPrescricaoRepository> _repositoryMock = new();
    private readonly PrescricaoService _sut;

    public PrescricaoServiceTests()
    {
        _sut = new PrescricaoService(_repositoryMock.Object);
    }

    [Fact]
    public void Patch_ApenasDuracao_MantemDosagemEFrequencia()
    {
        _repositoryMock.Setup(r => r.GetById(1)).Returns(new Prescricao("1 comprimido", "12/12h", 7, 1));

        var result = _sut.Patch(1, new PrescricaoPatchRequest { DuracaoDias = 14 });

        Assert.Equal(14, result!.DuracaoDias);
        Assert.Equal("1 comprimido", result.Dosagem);
        Assert.Equal("12/12h", result.Frequencia);
    }

    [Fact]
    public void Patch_DuracaoInvalida_LancaArgumentException()
    {
        _repositoryMock.Setup(r => r.GetById(1)).Returns(new Prescricao("1 comprimido", "12/12h", 7, 1));

        Assert.Throws<ArgumentException>(() => _sut.Patch(1, new PrescricaoPatchRequest { DuracaoDias = 0 }));
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
