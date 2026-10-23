using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class HistoricoPesoServiceTests
{
    private readonly Mock<IHistoricoPesoRepository> _repositoryMock = new();
    private readonly HistoricoPesoService _sut;

    public HistoricoPesoServiceTests()
    {
        _sut = new HistoricoPesoService(_repositoryMock.Object);
    }

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
}
