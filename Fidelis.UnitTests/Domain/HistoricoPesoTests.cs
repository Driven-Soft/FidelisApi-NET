using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class HistoricoPesoTests
{
    [Fact]
    public void Constructor_PesoPositivo_CriaHistorico()
    {
        var historico = new HistoricoPeso(12.5m, new DateTime(2026, 2, 1), 1, "Após dieta");

        Assert.Equal(12.5m, historico.PesoKg);
        Assert.Equal("Após dieta", historico.Observacao);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-3.2)]
    public void Constructor_PesoMenorOuIgualAZero_LancaArgumentException(double peso)
    {
        var act = () => new HistoricoPeso((decimal)peso, DateTime.UtcNow, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdatePeso_ValorValido_AtualizaPeso()
    {
        var historico = new HistoricoPeso(10m, DateTime.UtcNow, 1);

        historico.UpdatePeso(11.3m);

        Assert.Equal(11.3m, historico.PesoKg);
    }
}
