using System.Linq.Expressions;
using Fidelis.Application.DTOs;
using Fidelis.Domain.Entities;
using Xunit;

namespace Fidelis.UnitTests.Application;

public class QueryFiltersTests
{
    private static List<T> Aplicar<T>(IEnumerable<T> source, IReadOnlyList<Expression<Func<T, bool>>> filters) =>
        filters.Aggregate(source.AsQueryable(), (query, filter) => query.Where(filter)).ToList();

    private static readonly List<Pet> Pets =
    [
        new("Rex", "Cachorro", "Labrador", 'M', new DateTime(2020, 1, 1), "rex.jpg", 1),
        new("Mimi", "Gato", "Persa", 'F', new DateTime(2021, 1, 1), "mimi.jpg", 1),
        new("Thor", "Cachorro", "Husky", 'M', new DateTime(2019, 1, 1), "thor.jpg", 2, 5)
    ];

    [Fact]
    public void PetQuery_SemFiltros_RetornaTodos()
    {
        var query = new PetQueryParameters();

        var resultado = Aplicar(Pets, query.ToFilters());

        Assert.Equal(3, resultado.Count);
    }

    [Fact]
    public void PetQuery_FiltroPorEspecieSemDiferenciarMaiusculas_RetornaSomenteCachorros()
    {
        var query = new PetQueryParameters { Especie = "cachorro" };

        var resultado = Aplicar(Pets, query.ToFilters());

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, pet => Assert.Equal("Cachorro", pet.Especie));
    }

    [Fact]
    public void PetQuery_FiltrosCombinados_AplicaTodos()
    {
        var query = new PetQueryParameters { Especie = "cachorro", TutorId = 2, Sexo = 'm' };

        var resultado = Aplicar(Pets, query.ToFilters());

        var pet = Assert.Single(resultado);
        Assert.Equal("Thor", pet.Nome);
    }

    [Fact]
    public void PetQuery_FiltroPorClinica_RetornaSomentePetsDaClinica()
    {
        var query = new PetQueryParameters { ClinicaId = 5 };

        var resultado = Aplicar(Pets, query.ToFilters());

        Assert.Single(resultado);
    }

    [Fact]
    public void ConsultaQuery_FiltroPorPeriodo_RetornaConsultasNoIntervalo()
    {
        var consultas = new List<Consulta>
        {
            new(new DateTime(2026, 1, 10), "Rotina", 1, 1),
            new(new DateTime(2026, 2, 10), "Retorno", 1, 1),
            new(new DateTime(2026, 3, 10), "Emergência", 1, 2)
        };
        var query = new ConsultaQueryParameters { DataInicio = new DateTime(2026, 2, 1), DataFim = new DateTime(2026, 3, 31) };

        var resultado = Aplicar(consultas, query.ToFilters());

        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public void LembreteQuery_FiltroPorStatus_RetornaSomentePendentes()
    {
        var concluido = new Lembrete("Vacina", "V10", new DateTime(2026, 5, 1), 1, 1);
        concluido.Concluir();
        var lembretes = new List<Lembrete> { concluido, new("Banho", "Banho e tosa", new DateTime(2026, 5, 2), 1, 1) };
        var query = new LembreteQueryParameters { Status = 'p' };

        var resultado = Aplicar(lembretes, query.ToFilters());

        var lembrete = Assert.Single(resultado);
        Assert.Equal("Banho", lembrete.Tipo);
    }

    [Fact]
    public void TutorQuery_FiltroPorNomeParcial_RetornaCorrespondentes()
    {
        var tutores = new List<Tutor>
        {
            new("111", "Maria Silva", "maria@a.com", "123456", "11999999999", "Rua A"),
            new("222", "João Souza", "joao@a.com", "123456", "11999999999", "Rua B")
        };
        var query = new TutorQueryParameters { Nome = "silva" };

        var resultado = Aplicar(tutores, query.ToFilters());

        var tutor = Assert.Single(resultado);
        Assert.Equal("Maria Silva", tutor.Nome);
    }

    [Fact]
    public void VacinacaoQuery_FiltroProximaAte_RetornaVacinasAVencer()
    {
        var vacinacoes = new List<Vacinacao>
        {
            new("V10", new DateTime(2026, 1, 1), new DateTime(2026, 11, 1), 1, 1),
            new("Antirrábica", new DateTime(2026, 1, 1), new DateTime(2027, 6, 1), 1, 1)
        };
        var query = new VacinacaoQueryParameters { ProximaAte = new DateTime(2026, 12, 31) };

        var resultado = Aplicar(vacinacoes, query.ToFilters());

        var vacinacao = Assert.Single(resultado);
        Assert.Equal("V10", vacinacao.VacinaAplicada);
    }

    [Fact]
    public void HistoricoPesoQuery_FaixaDePeso_RetornaMedicoesNaFaixa()
    {
        var historicos = new List<HistoricoPeso>
        {
            new(8m, new DateTime(2026, 1, 1), 1),
            new(10m, new DateTime(2026, 2, 1), 1),
            new(14m, new DateTime(2026, 3, 1), 1)
        };
        var query = new HistoricoPesoQueryParameters { PesoMinimo = 9m, PesoMaximo = 12m };

        var resultado = Aplicar(historicos, query.ToFilters());

        var historico = Assert.Single(resultado);
        Assert.Equal(10m, historico.PesoKg);
    }
}
