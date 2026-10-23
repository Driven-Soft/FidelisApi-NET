using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class RegistroDiarioServiceTests
{
    private readonly Mock<IRegistroDiarioRepository> _repositoryMock = new();
    private readonly Mock<IPetRepository> _petRepositoryMock = new();
    private readonly RegistroDiarioService _sut;

    public RegistroDiarioServiceTests()
    {
        _sut = new RegistroDiarioService(_repositoryMock.Object, _petRepositoryMock.Object, NullLogger<RegistroDiarioService>.Instance);
    }

    private static RegistroDiarioRequest NovoRequest() => new()
    {
        PetId = 1,
        Titulo = "Passeio",
        Descricao = "Brincou no parque",
        Humor = "Feliz",
        Tags = ["passeio"],
        DataRegistro = new DateTime(2026, 10, 1)
    };

    private static RegistroDiario NovoRegistro() =>
        new(1, "Passeio", "Brincou no parque", "Feliz", ["passeio"], new DateTime(2026, 10, 1));

    private static Pet NovoPet() => new("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "rex.jpg", 1);

    [Fact]
    public void Create_PetExistente_GravaNoRepositorioNoSql()
    {
        _petRepositoryMock.Setup(r => r.GetById(1)).Returns(NovoPet());

        var result = _sut.Create(NovoRequest());

        Assert.Equal("Passeio", result.Titulo);
        Assert.Equal("Feliz", result.Humor);
        _repositoryMock.Verify(r => r.Add(It.Is<RegistroDiario>(x => x.PetId == 1)), Times.Once);
    }

    [Fact]
    public void Create_PetInexistente_LancaArgumentExceptionSemGravar()
    {
        _petRepositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        Assert.Throws<ArgumentException>(() => _sut.Create(NovoRequest()));
        _repositoryMock.Verify(r => r.Add(It.IsAny<RegistroDiario>()), Times.Never);
    }

    [Fact]
    public void Patch_ApenasTags_SubstituiTagsEMantemTitulo()
    {
        var registro = NovoRegistro();
        _repositoryMock.Setup(r => r.GetById("abc")).Returns(registro);

        var result = _sut.Patch("abc", new RegistroDiarioPatchRequest { Tags = ["Veterinario", "Vacina"] });

        Assert.Equal(new List<string> { "veterinario", "vacina" }, result!.Tags);
        Assert.Equal("Passeio", result.Titulo);
        Assert.NotNull(result.AtualizadoEm);
        _repositoryMock.Verify(r => r.Replace(registro), Times.Once);
    }

    [Fact]
    public void Update_RegistroInexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<string>())).Returns((RegistroDiario?)null);

        var result = _sut.Update("inexistente", new RegistroDiarioUpdateRequest());

        Assert.Null(result);
        _repositoryMock.Verify(r => r.Replace(It.IsAny<RegistroDiario>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Delete_RetornaResultadoDoRepositorio(bool removido)
    {
        _repositoryMock.Setup(r => r.Delete("abc")).Returns(removido);

        var result = _sut.Delete("abc");

        Assert.Equal(removido, result);
    }

    [Fact]
    public void GetAll_RepassaFiltrosEMapeiaResultado()
    {
        var query = new RegistroDiarioQueryParameters { PetId = 1, Tag = "passeio" };
        _repositoryMock.Setup(r => r.GetPaged(query)).Returns(new PagedResult<RegistroDiario>([NovoRegistro()], 1, 10, 1));

        var result = _sut.GetAll(query);

        var item = Assert.Single(result.Items);
        Assert.Equal(1, item.PetId);
    }
}
