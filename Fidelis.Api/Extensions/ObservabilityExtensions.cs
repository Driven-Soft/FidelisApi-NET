using Fidelis.Application.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Fidelis.Api.Extensions;

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
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(TelemetryConstants.ServiceName)
                    .AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(TelemetryConstants.MeterName)
                    .AddConsoleExporter();
            });

        return services;
    }
}
