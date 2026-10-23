using Fidelis.Application.Common;
using Xunit;

namespace Fidelis.UnitTests.Application;

public class FilterBuilderTests
{
    [Fact]
    public void AddIf_CondicaoFalsa_NaoAdicionaFiltro()
    {
        var filters = new FilterBuilder<int>()
            .AddIf(false, n => n > 10)
            .Build();

        Assert.Empty(filters);
    }

    [Fact]
    public void AddIf_VariasCondicoes_AdicionaSomenteAsVerdadeiras()
    {
        var filters = new FilterBuilder<int>()
            .AddIf(true, n => n > 1)
            .AddIf(false, n => n > 100)
            .AddIf(true, n => n % 2 == 0)
            .Build();

        var resultado = Enumerable.Range(1, 10).AsQueryable();
        resultado = filters.Aggregate(resultado, (query, filter) => query.Where(filter));

        Assert.Equal(2, filters.Count);
        Assert.Equal(new List<int> { 2, 4, 6, 8, 10 }, resultado.ToList());
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("   ", null)]
    [InlineData(" rex ", "REX")]
    public void TextFilter_Normalize_RetornaTextoEmMaiusculasOuNulo(string? entrada, string? esperado)
    {
        Assert.Equal(esperado, TextFilter.Normalize(entrada));
    }
}
