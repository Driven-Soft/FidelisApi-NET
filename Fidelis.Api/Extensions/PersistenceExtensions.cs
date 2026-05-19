using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Infrastructure.Persistence;
using Fidelis.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fidelis.Api.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        services.AddScoped<IComportamentoRepository, ComportamentoRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IExameRepository, ExameRepository>();
        services.AddScoped<IHistoricoPesoRepository, HistoricoPesoRepository>();
        services.AddScoped<ILembreteRepository, LembreteRepository>();
        services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IPrescricaoRepository, PrescricaoRepository>();
        services.AddScoped<IRecomendacaoRepository, RecomendacaoRepository>();
        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IVacinacaoRepository, VacinacaoRepository>();
        services.AddScoped<IVermifugacaoRepository, VermifugacaoRepository>();
        services.AddScoped<IVeterinarioRepository, VeterinarioRepository>();

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FidelisOracle")
            ?? throw new InvalidOperationException("FidelisOracle connection string is not configured.");

        services.AddDbContext<FidelisContext>(options =>
            options.UseOracle(connectionString));

        return services;
    }
}