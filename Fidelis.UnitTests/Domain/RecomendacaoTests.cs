using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class RecomendacaoTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaRecomendacao()
    {
        var recomendacao = new Recomendacao("Alimentação", "Reduzir petiscos", new DateTime(2026, 4, 1), 1);

        Assert.Equal("Alimentação", recomendacao.Tipo);
        Assert.Equal("Reduzir petiscos", recomendacao.Descricao);
    }

    [Fact]
    public void UpdateDescricao_Vazia_LancaArgumentException()
    {
        var recomendacao = new Recomendacao("Alimentação", "Reduzir petiscos", DateTime.UtcNow, 1);

        var act = () => recomendacao.UpdateDescricao(" ");

        Assert.Throws<ArgumentException>(act);
    }
}
