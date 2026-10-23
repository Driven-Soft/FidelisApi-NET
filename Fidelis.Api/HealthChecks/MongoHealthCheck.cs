using Fidelis.Infrastructure.Mongo;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fidelis.Api.HealthChecks;

public class MongoHealthCheck : IHealthCheck
{
    private readonly MongoDbContext _context;

    public MongoHealthCheck(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.PingAsync(cancellationToken);
            return HealthCheckResult.Healthy("MongoDB respondeu ao ping.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "MongoDB indisponível.", ex);
        }
    }
}
