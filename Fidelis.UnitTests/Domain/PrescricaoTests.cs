using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class PrescricaoTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaPrescricao()
    {
        var prescricao = new Prescricao("1 comprimido", "12/12h", 7, 1, "Após as refeições");

        Assert.Equal("1 comprimido", prescricao.Dosagem);
        Assert.Equal("12/12h", prescricao.Frequencia);
        Assert.Equal(7, prescricao.DuracaoDias);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_DuracaoInvalida_LancaArgumentException(int duracao)
    {
        var act = () => new Prescricao("1 comprimido", "12/12h", duracao, 1);

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Update_FrequenciaVazia_LancaArgumentException()
    {
        var prescricao = new Prescricao("1 comprimido", "12/12h", 7, 1);

        var act = () => prescricao.Update("2 comprimidos", "", 10, null);

        Assert.Throws<ArgumentException>(act);
    }
}
