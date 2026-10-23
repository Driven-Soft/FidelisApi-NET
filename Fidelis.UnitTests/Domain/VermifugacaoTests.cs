using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class VermifugacaoTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaVermifugacao()
    {
        var vermifugacao = new Vermifugacao("Drontal", new DateTime(2026, 1, 1), new DateTime(2026, 4, 1), 1, 2);

        Assert.Equal("Drontal", vermifugacao.Produto);
        Assert.Equal(2, vermifugacao.VeterinarioId);
    }

    [Fact]
    public void Constructor_ProdutoVazio_LancaArgumentException()
    {
        var act = () => new Vermifugacao(" ", new DateTime(2026, 1, 1), new DateTime(2026, 4, 1), 1, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateDatas_ProximaAntesDaAplicacao_LancaArgumentException()
    {
        var vermifugacao = new Vermifugacao("Drontal", new DateTime(2026, 1, 1), new DateTime(2026, 4, 1), 1, 1);

        var act = () => vermifugacao.UpdateDatas(new DateTime(2026, 6, 1), new DateTime(2026, 5, 1));

        Assert.Throws<ArgumentException>(act);
    }
}
