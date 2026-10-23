using Fidelis.Api.Auditoria;
using Fidelis.Api.HealthChecks;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Infrastructure.Mongo;
using Fidelis.Infrastructure.Mongo.Repositories;
using MongoDB.Driver;

namespace Fidelis.Api.Extensions;

public static class MongoExtensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(MongoSettings.SectionName).Get<MongoSettings>() ?? new MongoSettings();

        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new InvalidOperationException("MongoDb:ConnectionString não está configurada.");

        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.SectionName));

        services.AddSingleton<IMongoClient>(_ =>
        {
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(settings.ServerSelectionTimeoutSeconds);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.ServerSelectionTimeoutSeconds);
            return new MongoClient(clientSettings);
        });

        services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName));
        services.AddSingleton<MongoDbContext>();

        services.AddScoped<IRegistroDiarioRepository, RegistroDiarioRepository>();
        services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();

        services.AddSingleton<IAuditoriaQueue, AuditoriaQueue>();
        services.AddHostedService<AuditoriaBackgroundService>();
        services.AddHostedService<MongoIndexHostedService>();

        return services;
    }
}
