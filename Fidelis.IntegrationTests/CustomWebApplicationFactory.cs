using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Fidelis.Application.DTOs;
using Fidelis.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fidelis.IntegrationTests;

// WebApplicationFactory customizada que troca o Oracle configurado em
// Program.cs por um banco EF Core InMemory isolado por instancia de fabrica,
// permitindo rodar os testes de integracao sem depender de um Oracle real.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = "FidelisTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // O AddDbContext<FidelisContext>(options => options.UseOracle(...)) original
            // (em PersistenceExtensions) nao registra apenas o DbContextOptions<FidelisContext>:
            // desde o EF Core 5+, ele tambem registra a acao de configuracao como
            // IDbContextOptionsConfiguration<FidelisContext>. Se removermos so o
            // DbContextOptions<FidelisContext> e adicionarmos outro AddDbContext por cima,
            // as DUAS configuracoes (Oracle + InMemory) acabam sendo aplicadas ao montar as
            // options finais, e o EF Core lanca InvalidOperationException por ter dois
            // provedores configurados ao mesmo tempo. Por isso removemos TODO descriptor
            // relacionado ao FidelisContext antes de registrar o InMemory.
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<FidelisContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(FidelisContext) ||
                    (d.ServiceType.FullName != null &&
                     d.ServiceType.FullName.Contains("DbContextOptionsConfiguration")))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Registra o DbContext usando um provedor InMemory, exclusivo desta execucao de testes.
            services.AddDbContext<FidelisContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }

    // Cria um novo escopo de DI e devolve o FidelisContext, util para preparar
    // (seed) dados diretamente no banco antes de exercitar os endpoints HTTP.
    public FidelisContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<FidelisContext>();
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Cadastra um tutor de teste (endpoint público) e faz login em seguida,
    /// devolvendo um HttpClient já com o header "Authorization: Bearer {token}"
    /// configurado, pronto para chamar os endpoints protegidos por [Authorize].
    /// </summary>
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
}