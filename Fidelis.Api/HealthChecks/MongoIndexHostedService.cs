using Fidelis.Infrastructure.Mongo;

namespace Fidelis.Api.HealthChecks;

public class MongoIndexHostedService : BackgroundService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<MongoIndexHostedService> _logger;

    public MongoIndexHostedService(MongoDbContext context, ILogger<MongoIndexHostedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _context.EnsureIndexesAsync(stoppingToken);
            _logger.LogInformation("Índices do MongoDB verificados com sucesso");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível criar os índices do MongoDB");
        }
    }
}
