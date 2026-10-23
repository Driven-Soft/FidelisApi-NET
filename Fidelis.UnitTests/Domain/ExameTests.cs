using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class ExameTests
{
    [Fact]
    public void Constructor_SemResultado_CriaExameComResultadoNulo()
    {
        var exame = new Exame("Hemograma", "Exame de sangue completo", new DateTime(2026, 3, 1), 1);

        Assert.Equal("Hemograma", exame.Tipo);
        Assert.Null(exame.Resultado);
    }

    [Fact]
    public void UpdateResultado_ComEspacos_ArmazenaResultadoNormalizado()
    {
        var exame = new Exame("Hemograma", "Exame de sangue completo", new DateTime(2026, 3, 1), 1);

        exame.UpdateResultado("  Normal  ");

        Assert.Equal("Normal", exame.Resultado);
    }

    [Fact]
    public void Constructor_TipoVazio_LancaArgumentException()
    {
        var act = () => new Exame("", "Descrição", DateTime.UtcNow, 1);

        Assert.Throws<ArgumentException>(act);
    }
}
