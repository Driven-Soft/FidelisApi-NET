using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class LembreteTests
{
    private static Lembrete NovoLembrete() => new("Vacina", "Reforço da V10", new DateTime(2026, 12, 1), 1, 1);

    [Fact]
    public void Constructor_DadosValidos_CriaLembretePendente()
    {
        var lembrete = NovoLembrete();

        Assert.Equal('P', lembrete.Status);
        Assert.Equal("Vacina", lembrete.Tipo);
    }

    [Fact]
    public void Concluir_LembretePendente_AlteraStatusParaConcluido()
    {
        var lembrete = NovoLembrete();

        lembrete.Concluir();

        Assert.Equal('C', lembrete.Status);
    }

    [Fact]
    public void Cancelar_LembretePendente_AlteraStatusParaCancelado()
    {
        var lembrete = NovoLembrete();

        lembrete.Cancelar();

        Assert.Equal('X', lembrete.Status);
    }

    [Fact]
    public void Constructor_DescricaoVazia_LancaArgumentException()
    {
        var act = () => new Lembrete("Vacina", "", DateTime.UtcNow, 1, 1);

        Assert.Throws<ArgumentException>(act);
    }
}
