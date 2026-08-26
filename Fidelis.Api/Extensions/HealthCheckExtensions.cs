using Fidelis.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fidelis.Api.Extensions;


public static class HealthCheckExtensions
{
    public static IServiceCollection AddFidelisHealthChecks(this IServiceCollection services)
    {
 
        services.AddHealthChecks()
            .AddCheck("self", CheckSelf, tags: new[] { "live" })
            .AddDbContextCheck<FidelisContext>("oracle-database", tags: new[] { "ready" });

        return services;
    }

    private static HealthCheckResult CheckSelf()
    {
        return HealthCheckResult.Healthy("A aplicacao esta em execucao.");
    }
}
