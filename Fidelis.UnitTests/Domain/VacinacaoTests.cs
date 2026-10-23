using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class VacinacaoTests
{
    [Fact]
    public void Constructor_DatasValidas_CriaVacinacao()
    {
        var aplicacao = new DateTime(2026, 1, 1);
        var proxima = new DateTime(2027, 1, 1);

        var vacinacao = new Vacinacao("V10", aplicacao, proxima, 1, 2);

        Assert.Equal("V10", vacinacao.VacinaAplicada);
        Assert.Equal(aplicacao, vacinacao.DataAplicacao);
        Assert.Equal(proxima, vacinacao.DataProxima);
    }

    [Fact]
    public void Constructor_DataProximaAnteriorAAplicacao_LancaArgumentException()
    {
        var act = () => new Vacinacao("Antirrábica", new DateTime(2026, 5, 1), new DateTime(2026, 4, 1), 1, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateDatas_DatasIguais_LancaArgumentException()
    {
        var vacinacao = new Vacinacao("V8", new DateTime(2026, 1, 1), new DateTime(2026, 12, 1), 1, 1);
        var mesmaData = new DateTime(2026, 3, 1);

        var act = () => vacinacao.UpdateDatas(mesmaData, mesmaData);

        Assert.Throws<ArgumentException>(act);
    }
}
