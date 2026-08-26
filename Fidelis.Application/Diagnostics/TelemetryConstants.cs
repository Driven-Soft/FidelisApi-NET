namespace Fidelis.Application.Diagnostics;

/// <summary>
/// Nomes centrais usados para registrar as fontes de tracing (ActivitySource)
/// e de métricas (Meter) da aplicação junto ao OpenTelemetry.
/// </summary>
public static class TelemetryConstants
{
    public const string ServiceName = "Fidelis.Api";
    public const string MeterName = "Fidelis.Metrics";
}
