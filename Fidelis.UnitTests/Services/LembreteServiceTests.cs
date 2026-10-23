using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class LembreteServiceTests
{
    private readonly Mock<ILembreteRepository> _repositoryMock = new();
    private readonly LembreteService _sut;

    public LembreteServiceTests()
    {
        _sut = new LembreteService(_repositoryMock.Object);
    }

    [Theory]
    [InlineData('C', 'C')]
    [InlineData('X', 'X')]
    [InlineData('Z', 'P')]
    public void Patch_Status_AplicaTransicaoEsperada(char statusInformado, char statusEsperado)
    {
        _repositoryMock.Setup(r => r.GetById(1)).Returns(new Lembrete("Vacina", "V10", new DateTime(2026, 12, 1), 1, 1));

        var result = _sut.Patch(1, new LembretePatchRequest { Status = statusInformado });

        Assert.Equal(statusEsperado, result!.Status);
    }

    [Fact]
    public void Create_RequestValido_CriaLembretePendente()
    {
        var request = new LembreteRequest
        {
            Tipo = "Banho",
            Descricao = "Banho e tosa",
            DataPrevista = new DateTime(2026, 11, 10),
            TutorId = 1,
            PetId = 1
        };

        var result = _sut.Create(request);

        Assert.Equal('P', result.Status);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }
}
