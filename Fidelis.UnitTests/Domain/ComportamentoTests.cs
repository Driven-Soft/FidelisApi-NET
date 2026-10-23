using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class ComportamentoTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaComportamento()
    {
        var comportamento = new Comportamento("  Late para visitas  ", new DateTime(2026, 2, 2), 1);

        Assert.Equal("Late para visitas", comportamento.Descricao);
        Assert.Equal(1, comportamento.PetId);
    }

    [Fact]
    public void Constructor_DescricaoVazia_LancaArgumentException()
    {
        var act = () => new Comportamento("", DateTime.UtcNow, 1);

        Assert.Throws<ArgumentException>(act);
    }
}
