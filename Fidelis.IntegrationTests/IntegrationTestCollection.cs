using Xunit;

namespace Fidelis.IntegrationTests;

/// <summary>
/// Collection Fixture do xUnit: garante que uma única instância de
/// <see cref="CustomWebApplicationFactory"/> (e, portanto, do host + banco
/// InMemory) seja compartilhada entre todas as classes de teste marcadas com
/// [Collection("Integration Tests")], evitando o custo de subir a aplicação
/// inteira a cada classe de teste.
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
