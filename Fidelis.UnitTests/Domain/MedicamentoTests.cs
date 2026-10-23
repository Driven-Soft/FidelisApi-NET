using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Domain;

public class MedicamentoTests
{
    [Fact]
    public void Constructor_DadosValidos_CriaMedicamento()
    {
        var medicamento = new Medicamento(" Amoxicilina ", "Antibiótico", 1);

        Assert.Equal("Amoxicilina", medicamento.Nome);
        Assert.Equal(1, medicamento.PrescricaoId);
    }

    [Fact]
    public void Update_NomeVazio_LancaArgumentException()
    {
        var medicamento = new Medicamento("Amoxicilina", "Antibiótico", 1);

        var act = () => medicamento.Update("", "Antibiótico");

        Assert.Throws<ArgumentException>(act);
    }
}
