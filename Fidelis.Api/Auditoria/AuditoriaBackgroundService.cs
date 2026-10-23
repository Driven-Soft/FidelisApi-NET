using Fidelis.Application.Services.Interfaces;

namespace Fidelis.Api.Auditoria;

public class AuditoriaBackgroundService : BackgroundService
{
    private readonly IAuditoriaQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditoriaBackgroundService> _logger;

    public AuditoriaBackgroundService(
        IAuditoriaQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditoriaBackgroundService> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var log in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                scope.ServiceProvider.GetRequiredService<IAuditoriaService>().Registrar(log);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao gravar auditoria de {Metodo} {Caminho} no MongoDB", log.Metodo, log.Caminho);
            }
        }
    }
}
