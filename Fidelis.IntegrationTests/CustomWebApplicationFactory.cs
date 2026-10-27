using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Api.Extensions;
using Fidelis.Api.HealthChecks;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Clients;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.IntegrationTests.Fakes;
using Fidelis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Fidelis.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly string _databaseName = "FidelisTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<FidelisContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(FidelisContext) ||
                    (d.ServiceType.FullName != null &&
                     d.ServiceType.FullName.Contains("DbContextOptionsConfiguration")))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            services.AddDbContext<FidelisContext>(options => options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<IRegistroDiarioRepository>();
            services.RemoveAll<IAuditoriaRepository>();
            services.RemoveAll<IFidelisAiClient>();
            services.AddSingleton<IRegistroDiarioRepository, InMemoryRegistroDiarioRepository>();
            services.AddSingleton<IAuditoriaRepository, InMemoryAuditoriaRepository>();
            services.AddSingleton<IFidelisAiClient, InMemoryFidelisAiClient>();

            var mongoIndexService = services.FirstOrDefault(d => d.ImplementationType == typeof(MongoIndexHostedService));
            if (mongoIndexService is not null)
                services.Remove(mongoIndexService);

            services.Configure<HealthCheckServiceOptions>(options =>
            {
                var mongoCheck = options.Registrations.FirstOrDefault(r => r.Name == HealthCheckExtensions.MongoCheckName);
                if (mongoCheck is not null)
                    options.Registrations.Remove(mongoCheck);
            });
        });
    }

    public FidelisContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<FidelisContext>();
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var cpf = Guid.NewGuid().ToString("N")[..11];
        var email = $"{Guid.NewGuid():N}@example.com";
        const string senha = "123456";

        await client.PostAsJsonAsync("/api/Tutor", new TutorRequest
        {
            Cpf = cpf,
            Nome = "Usuario de Teste",
            Email = email,
            Senha = senha,
            Telefone = "11988476632",
            Endereco = "Rua de Teste, 1"
        });

        var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginRequest
        {
            Email = email,
            Senha = senha
        });

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);

        return client;
    }

    public int SeedTutor()
    {
        using var context = CreateDbContext();
        var tutor = new Tutor(
            Guid.NewGuid().ToString("N")[..11],
            "Tutor de Apoio",
            $"{Guid.NewGuid():N}@example.com",
            "123456",
            "11988476632",
            "Rua de Apoio, 1");

        context.Tutores.Add(tutor);
        context.SaveChanges();
        return tutor.Id;
    }

    public int SeedPet(string nome = "Rex", string especie = "Cachorro", int? tutorId = null)
    {
        var donoId = tutorId ?? SeedTutor();
        using var context = CreateDbContext();
        var pet = new Pet(nome, especie, "SRD", 'M', new DateTime(2020, 1, 1), "https://example.com/pet.jpg", donoId);

        context.Pets.Add(pet);
        context.SaveChanges();
        return pet.Id;
    }
}
