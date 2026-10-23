using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class ConsultaTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaConsulta()
    {
        var dataHora = new DateTime(2026, 5, 10, 14, 0, 0);

        var consulta = new Consulta(dataHora, "Rotina", 2, 3, "Saudável", "Sem observações");

        Assert.Equal(dataHora, consulta.DataHora);
        Assert.Equal("Rotina", consulta.Tipo);
        Assert.Equal(2, consulta.VeterinarioId);
        Assert.Equal(3, consulta.PetId);
        Assert.Null(consulta.DataRetorno);
    }

    [Fact]
    public void Constructor_TipoVazio_LancaArgumentException()
    {
        var act = () => new Consulta(DateTime.UtcNow, "", 1, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Update_DadosValidos_AtualizaCamposClinicos()
    {
        var consulta = new Consulta(DateTime.UtcNow, "Rotina", 1, 1);
        var retorno = new DateTime(2026, 6, 1);

        consulta.Update("Retorno", "  Otite  ", " Usar colírio ", retorno);

        Assert.Equal("Retorno", consulta.Tipo);
        Assert.Equal("Otite", consulta.Diagnostico);
        Assert.Equal("Usar colírio", consulta.Observacoes);
        Assert.Equal(retorno, consulta.DataRetorno);
    }
}
