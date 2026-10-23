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

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _repositoryMock;
    private readonly PetService _sut;

    public PetServiceTests()
    {
        _repositoryMock = new Mock<IPetRepository>();
        _sut = new PetService(_repositoryMock.Object, NullLogger<PetService>.Instance);
    }

    private static PetRequest NovoRequest(string nome = "Rex") => new()
    {
        Nome = nome,
        Especie = "Cachorro",
        Raca = "Labrador",
        Sexo = 'M',
        DataNascimento = new DateTime(2020, 1, 1),
        FotoUrl = "foto.jpg",
        TutorId = 1
    };

    [Fact]
    public void Create_RequestValido_AdicionaPetERetornaResponse()
    {
        var result = _sut.Create(NovoRequest());

        Assert.Equal("Rex", result.Nome);
        Assert.Equal("Cachorro", result.Especie);
        _repositoryMock.Verify(r => r.Add(It.IsAny<Pet>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Create_SexoInvalido_LancaArgumentExceptionENaoPersiste()
    {
        var request = NovoRequest();
        request.Sexo = 'X';

        Assert.Throws<ArgumentException>(() => _sut.Create(request));
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void GetById_PetExistente_RetornaPetResponse()
    {
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        var result = _sut.GetById(1);

        Assert.NotNull(result);
        Assert.Equal("Rex", result!.Nome);
    }

    [Fact]
    public void GetById_PetInexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        var result = _sut.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_RepositorioComPets_RetornaPaginaComResponsesEMetadados()
    {
        var pets = new List<Pet>
        {
            new("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto1.jpg", 1),
            new("Mimi", "Gato", "Persa", 'F', new DateTime(2021, 3, 1), "foto2.jpg", 1)
        };
        var query = new PetQueryParameters { Page = 1, PageSize = 2 };
        _repositoryMock
            .Setup(r => r.GetPaged(query, It.IsAny<IEnumerable<Expression<Func<Pet, bool>>>>()))
            .Returns(new PagedResult<Pet>(pets, 1, 2, 5));

        var result = _sut.GetAll(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNext);
        Assert.Equal("Rex", result.Items[0].Nome);
    }

    [Fact]
    public void Update_PetInexistente_RetornaNullENaoChamaSaveChanges()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        var result = _sut.Update(999, NovoRequest());

        Assert.Null(result);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Update_PetExistente_AtualizaERetornaResponseAtualizado()
    {
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        var result = _sut.Update(1, NovoRequest("Rex Atualizado"));

        Assert.NotNull(result);
        Assert.Equal("Rex Atualizado", result!.Nome);
        _repositoryMock.Verify(r => r.Update(pet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Patch_ApenasRacaInformada_MantemDemaisCampos()
    {
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        var result = _sut.Patch(1, new PetPatchRequest { Raca = "Golden" });

        Assert.Equal("Golden", result!.Raca);
        Assert.Equal("Rex", result.Nome);
    }

    [Fact]
    public void Delete_PetExistente_RemoveERetornaTrue()
    {
        var pet = new Pet("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "foto.jpg", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(pet);

        var result = _sut.Delete(1);

        Assert.True(result);
        _repositoryMock.Verify(r => r.Delete(pet), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_PetInexistente_RetornaFalseSemChamarRepositorio()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Pet?)null);

        var result = _sut.Delete(999);

        Assert.False(result);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Pet>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Never);
    }
}
