using Fidelis.Application.Common;
using Xunit;

namespace Fidelis.UnitTests.Application;

public class PagedResultTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(95, 20, 5)]
    public void TotalPages_DeAcordoComTotalDeItens_CalculaCorretamente(int totalItems, int pageSize, int esperado)
    {
        var result = new PagedResult<int>([], 1, pageSize, totalItems);

        Assert.Equal(esperado, result.TotalPages);
    }

    [Fact]
    public void Navegacao_PaginaIntermediaria_PossuiAnteriorEProxima()
    {
        var result = new PagedResult<int>([1, 2], 2, 2, 6);

        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
    }

    [Fact]
    public void Navegacao_UltimaPagina_NaoPossuiProxima()
    {
        var result = new PagedResult<int>([5, 6], 3, 2, 6);

        Assert.True(result.HasPrevious);
        Assert.False(result.HasNext);
    }

    [Fact]
    public void Map_ConverteItensEMantemMetadados()
    {
        var result = new PagedResult<int>([1, 2, 3], 2, 3, 9);

        var mapped = result.Map(n => $"item-{n}");

        Assert.Equal(new List<string> { "item-1", "item-2", "item-3" }, mapped.Items);
        Assert.Equal(2, mapped.Page);
        Assert.Equal(3, mapped.PageSize);
        Assert.Equal(9, mapped.TotalItems);
    }
}
