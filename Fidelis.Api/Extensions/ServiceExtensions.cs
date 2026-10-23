using Fidelis.Application.Services.Implementations;
using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClinicaService, ClinicaService>();
        services.AddScoped<IComportamentoService, ComportamentoService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IExameService, ExameService>();
        services.AddScoped<IHistoricoPesoService, HistoricoPesoService>();
        services.AddScoped<ILembreteService, LembreteService>();
        services.AddScoped<IMedicamentoService, MedicamentoService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IPrescricaoService, PrescricaoService>();
        services.AddScoped<IRecomendacaoService, RecomendacaoService>();
        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IVacinacaoService, VacinacaoService>();
        services.AddScoped<IVermifugacaoService, VermifugacaoService>();
        services.AddScoped<IVeterinarioService, VeterinarioService>();
        services.AddScoped<IRegistroDiarioService, RegistroDiarioService>();
        services.AddScoped<IAuditoriaService, AuditoriaService>();

        return services;
    }
}
