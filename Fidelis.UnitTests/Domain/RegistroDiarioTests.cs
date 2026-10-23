using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class RegistroDiarioTests
{
    private static RegistroDiario NovoRegistro(IEnumerable<string>? tags = null) =>
        new(1, "Passeio", "Brincou no parque", "feliz", tags ?? ["passeio"], new DateTime(2026, 10, 1));

    [Fact]
    public void Constructor_DadosValidos_NormalizaHumorETags()
    {
        var registro = NovoRegistro([" Passeio ", "ENERGIA", "passeio", ""]);

        Assert.Equal("Feliz", registro.Humor);
        Assert.Equal(new List<string> { "passeio", "energia" }, registro.Tags);
        Assert.Null(registro.AtualizadoEm);
    }

    [Fact]
    public void Constructor_HumorNaoPermitido_LancaArgumentException()
    {
        var act = () => new RegistroDiario(1, "Passeio", "Brincou", "Eufórico", null, new DateTime(2026, 10, 1));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_PetInvalido_LancaArgumentException()
    {
        var act = () => new RegistroDiario(0, "Passeio", "Brincou", "Feliz", null, new DateTime(2026, 10, 1));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_MaisTagsQueOPermitido_LancaArgumentException()
    {
        var tags = Enumerable.Range(1, RegistroDiario.MaxTags + 1).Select(i => $"tag{i}");

        var act = () => NovoRegistro(tags);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Constructor_DataNoFuturo_LancaArgumentException()
    {
        var act = () => new RegistroDiario(1, "Passeio", "Brincou", "Feliz", null, DateTime.UtcNow.AddDays(10));

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void UpdateHumor_ValorValido_AtualizaHumorEDataDeAtualizacao()
    {
        var registro = NovoRegistro();

        registro.UpdateHumor("ansioso");

        Assert.Equal("Ansioso", registro.Humor);
        Assert.NotNull(registro.AtualizadoEm);
    }

    [Fact]
    public void Update_TituloVazio_LancaArgumentException()
    {
        var registro = NovoRegistro();

        var act = () => registro.Update("", "Descrição", "Calmo", null, new DateTime(2026, 10, 2));

        Assert.Throws<ArgumentException>(act);
    }
}
