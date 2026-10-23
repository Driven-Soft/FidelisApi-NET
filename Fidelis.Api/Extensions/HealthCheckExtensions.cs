using Fidelis.Api.HealthChecks;
using Fidelis.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fidelis.Api.Extensions;

public static class HealthCheckExtensions
{
    public const string LiveTag = "live";
    public const string ReadyTag = "ready";
    public const string OracleCheckName = "oracle-database";
    public const string MongoCheckName = "mongodb";

    public static IServiceCollection AddFidelisHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("A aplicação está em execução."), tags: [LiveTag])
            .AddDbContextCheck<FidelisContext>(OracleCheckName, tags: [ReadyTag])
            .AddCheck<MongoHealthCheck>(MongoCheckName, tags: [ReadyTag]);

        return services;
    }
}
