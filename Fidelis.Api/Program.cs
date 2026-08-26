using Fidelis.Api.Exceptions;
using Fidelis.Api.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

// Configuração do Serilog (log estruturado, enriquecido com o contexto de cada requisição).
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando a Fidelis API...");

    var builder = WebApplication.CreateBuilder(args);

    // Substitui o logger padrão do ASP.NET Core pelo Serilog.
    builder.Host.UseSerilog();

    // Todos os endpoints exigem [Authorize] por padrão; use [AllowAnonymous]
    // nos endpoints públicos (login, cadastro de tutor).
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddSwagger();
    builder.Services.AddServices();
    builder.Services.AddRepositories();
    builder.Services.AddDbContext(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Observabilidade: tracing distribuído e métricas via OpenTelemetry.
    builder.Services.AddObservability();

    // Health Checks: liveness ("self") e readiness (conectividade com o Oracle).
    builder.Services.AddFidelisHealthChecks();

    var app = builder.Build();

    // Log estruturado de cada requisição HTTP (método, rota, status, tempo de resposta).
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fidelis API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Liveness: a aplicação está viva/travada? Não verifica dependências externas.
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    });

    // Readiness: a aplicação está pronta para receber tráfego (banco disponível)?
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    // Relatório completo (todas as verificações), útil para dashboards e debug manual.
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}

// Necessário para que o WebApplicationFactory<Program>, usado nos testes de
// integração, consiga localizar e reaproveitar o pipeline desta aplicação.
public partial class Program { }
