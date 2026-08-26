using Fidelis.Application.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fidelis.Api.Extensions;

/// <summary>
/// Configura o OpenTelemetry para tracing distribuído e métricas de aplicação.
/// Ambos os sinais, por padrão, são exportados para o console (útil em ambiente
/// de desenvolvimento/estudo). Em produção, o ConsoleExporter pode ser trocado
/// por OTLP apontando para um coletor (Grafana, Jaeger, Prometheus, etc.).
/// </summary>
public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(TelemetryConstants.ServiceName);

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation() // Captura requisições HTTP de entrada (API)
                    .AddHttpClientInstrumentation() // Captura chamadas HTTP de saída
                    .AddSource(TelemetryConstants.ServiceName) // Spans customizados criados nos Services
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation() // Métricas padrão do ASP.NET Core (latência, requests/s...)
                    .AddHttpClientInstrumentation()
                    .AddMeter(TelemetryConstants.MeterName) // Counters customizados criados nos Services
                    .AddConsoleExporter();
            });

        return services;
    }
}
