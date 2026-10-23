using System.ComponentModel.DataAnnotations;
using Fidelis.Application.Common;
using Xunit;

namespace Fidelis.UnitTests.Application;

public class QueryParametersTests
{
    private static List<ValidationResult> Validar(QueryParameters parameters)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(parameters, new ValidationContext(parameters), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Constructor_SemValores_UsaPaginaUmETamanhoPadrao()
    {
        var parameters = new QueryParameters();

        Assert.Equal(1, parameters.Page);
        Assert.Equal(QueryParameters.DefaultPageSize, parameters.PageSize);
        Assert.False(parameters.IsDescending());
        Assert.Equal(0, parameters.Skip());
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 25, 50)]
    public void Skip_PaginaETamanho_CalculaDeslocamentoCorreto(int page, int pageSize, int esperado)
    {
        var parameters = new QueryParameters { Page = page, PageSize = pageSize };

        Assert.Equal(esperado, parameters.Skip());
    }

    [Theory]
    [InlineData("desc", true)]
    [InlineData("DESC", true)]
    [InlineData("asc", false)]
    [InlineData(null, false)]
    public void IsDescending_DeAcordoComDirecao_RetornaValorEsperado(string? direcao, bool esperado)
    {
        var parameters = new QueryParameters { SortDirection = direcao };

        Assert.Equal(esperado, parameters.IsDescending());
    }

    [Fact]
    public void Validacao_PageSizeAcimaDoMaximo_RetornaErro()
    {
        var parameters = new QueryParameters { PageSize = QueryParameters.MaxPageSize + 1 };

        var erros = Validar(parameters);

        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(QueryParameters.PageSize)));
    }

    [Fact]
    public void Validacao_DirecaoInvalida_RetornaErro()
    {
        var parameters = new QueryParameters { SortDirection = "lateral" };

        var erros = Validar(parameters);

        Assert.Contains(erros, e => e.MemberNames.Contains(nameof(QueryParameters.SortDirection)));
    }

    [Fact]
    public void Validacao_ParametrosValidos_NaoRetornaErros()
    {
        var parameters = new QueryParameters { Page = 2, PageSize = 50, SortBy = "nome", SortDirection = "desc" };

        var erros = Validar(parameters);

        Assert.Empty(erros);
    }
}
