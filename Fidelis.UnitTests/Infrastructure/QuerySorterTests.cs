using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Repositories;
using Xunit;

namespace Fidelis.UnitTests.Infrastructure;

public class QuerySorterTests
{
    private static readonly IQueryable<Pet> Pets = new List<Pet>
    {
        new("Thor", "Cachorro", "Husky", 'M', new DateTime(2019, 1, 1), "thor.jpg", 1),
        new("Amora", "Gato", "Siamês", 'F', new DateTime(2022, 1, 1), "amora.jpg", 1),
        new("Mel", "Cachorro", "Poodle", 'F', new DateTime(2020, 1, 1), "mel.jpg", 1)
    }.AsQueryable();

    [Fact]
    public void Apply_OrdenacaoAscendentePorNome_OrdenaAlfabeticamente()
    {
        var resultado = QuerySorter.Apply(Pets, "nome", descending: false).Select(p => p.Nome).ToList();

        Assert.Equal(new List<string> { "Amora", "Mel", "Thor" }, resultado);
    }

    [Fact]
    public void Apply_OrdenacaoDescendentePorData_OrdenaDoMaisNovoParaOMaisVelho()
    {
        var resultado = QuerySorter.Apply(Pets, "DataNascimento", descending: true).Select(p => p.Nome).ToList();

        Assert.Equal(new List<string> { "Amora", "Mel", "Thor" }, resultado);
    }

    [Fact]
    public void Apply_SemCampo_OrdenaPorId()
    {
        var resultado = QuerySorter.Apply(Pets, null, descending: false).ToList();

        Assert.Equal(3, resultado.Count);
    }

    [Theory]
    [InlineData("campoInexistente")]
    [InlineData("Tutor")]
    [InlineData("Consultas")]
    public void Apply_CampoInvalidoOuNavegacao_LancaArgumentException(string campo)
    {
        var act = () => QuerySorter.Apply(Pets, campo, descending: false);

        Assert.Throws<ArgumentException>(act);
    }
}
