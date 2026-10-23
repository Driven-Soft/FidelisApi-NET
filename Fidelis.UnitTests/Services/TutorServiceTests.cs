using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _repositoryMock;
    private readonly TutorService _sut;

    public TutorServiceTests()
    {
        _repositoryMock = new Mock<ITutorRepository>();
        _sut = new TutorService(_repositoryMock.Object, NullLogger<TutorService>.Instance);
    }

    private static TutorRequest NovoRequest() => new()
    {
        Cpf = "12345678900",
        Nome = "Felipe",
        Email = "felipe@example.com",
        Senha = "123456",
        Telefone = "11988476632",
        Endereco = "Rua Exemplo, 123"
    };

    [Fact]
    public void Create_RequestValido_AdicionaTutorERetornaResponse()
    {
        _repositoryMock.Setup(r => r.ExistsByEmail("felipe@example.com")).Returns(false);

        var result = _sut.Create(NovoRequest());

        Assert.Equal("Felipe", result.Nome);
        Assert.Equal("felipe@example.com", result.Email);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Create_EmailJaCadastrado_LancaInvalidOperationExceptionENaoAdiciona()
    {
        _repositoryMock.Setup(r => r.ExistsByEmail(It.IsAny<string>())).Returns(true);

        Assert.Throws<InvalidOperationException>(() => _sut.Create(NovoRequest()));
        _repositoryMock.Verify(r => r.Add(It.IsAny<Tutor>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetAll_ComFiltro_RepassaQueryAoRepositorio()
    {
        var query = new TutorQueryParameters { Nome = "felipe", Page = 2, PageSize = 5 };
        var tutores = new List<Tutor> { new("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua") };
        _repositoryMock
            .Setup(r => r.GetPaged(query, It.IsAny<IEnumerable<Expression<Func<Tutor, bool>>>>()))
            .Returns(new PagedResult<Tutor>(tutores, 2, 5, 6));

        var result = _sut.GetAll(query);

        Assert.Single(result.Items);
        Assert.Equal(2, result.Page);
        Assert.False(result.HasNext);
        _repositoryMock.Verify(r => r.GetPaged(query, It.IsAny<IEnumerable<Expression<Func<Tutor, bool>>>>()), Times.Once);
    }

    [Fact]
    public void GetById_TutorInexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Tutor?)null);

        var result = _sut.GetById(42);

        Assert.Null(result);
    }

    [Fact]
    public void Patch_ApenasNomeInformado_MantemDemaisCamposOriginais()
    {
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");
        _repositoryMock.Setup(r => r.GetById(1)).Returns(tutor);

        var result = _sut.Patch(1, new TutorPatchRequest { Nome = "Felipe Bezerra" });

        Assert.NotNull(result);
        Assert.Equal("Felipe Bezerra", result!.Nome);
        Assert.Equal("felipe@example.com", result.Email);
        Assert.Equal("12345678900", result.Cpf);
    }

    [Fact]
    public void Delete_TutorExistente_RemoveERetornaTrue()
    {
        var tutor = new Tutor("12345678900", "Felipe", "felipe@example.com", "123456", "11988476632", "Rua Exemplo");
        _repositoryMock.Setup(r => r.GetById(1)).Returns(tutor);

        var result = _sut.Delete(1);

        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(tutor), Times.Once);
    }

    [Fact]
    public void Delete_TutorInexistente_RetornaFalse()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Tutor?)null);

        var result = _sut.Delete(999);

        Assert.False(result);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
