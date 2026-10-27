using Fidelis.Application.Interfaces.Clients;
using Fidelis.Application.Settings;
using Fidelis.Infrastructure.Clients;

namespace Fidelis.Api.Extensions;

public static class FidelisAiExtensions
{
    public static IServiceCollection AddFidelisAi(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(FidelisAiSettings.SectionName).Get<FidelisAiSettings>() ?? new FidelisAiSettings();

        services.Configure<FidelisAiSettings>(configuration.GetSection(FidelisAiSettings.SectionName));

        services.AddHttpClient<IFidelisAiClient, FidelisAiClient>(client =>
        {
            if (!string.IsNullOrWhiteSpace(settings.BaseUrl))
                client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");

            client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
                client.DefaultRequestHeaders.Add(FidelisAiClient.ApiKeyHeader, settings.ApiKey);
        });

        return services;
    }
}
