using Fidelis.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fidelis.Infrastructure.Mongo;

public class MongoDbContext
{
    public const string RegistrosDiarioCollectionName = "registros_diario";
    public const string LogsAuditoriaCollectionName = "logs_auditoria";

    public IMongoDatabase Database { get; }

    public MongoDbContext(IMongoDatabase database)
    {
        MongoMappings.Register();
        Database = database;
    }

    public IMongoCollection<RegistroDiario> RegistrosDiario =>
        Database.GetCollection<RegistroDiario>(RegistrosDiarioCollectionName);

    public IMongoCollection<LogAuditoria> LogsAuditoria =>
        Database.GetCollection<LogAuditoria>(LogsAuditoriaCollectionName);

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        await Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
    }

    public async Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var registroIndexes = new[]
        {
            new CreateIndexModel<RegistroDiario>(
                Builders<RegistroDiario>.IndexKeys.Ascending(r => r.PetId).Descending(r => r.DataRegistro),
                new CreateIndexOptions { Name = "ix_pet_data" }),
            new CreateIndexModel<RegistroDiario>(
                Builders<RegistroDiario>.IndexKeys.Ascending(r => r.Tags),
                new CreateIndexOptions { Name = "ix_tags" })
        };

        await RegistrosDiario.Indexes.CreateManyAsync(registroIndexes, cancellationToken);

        await LogsAuditoria.Indexes.CreateOneAsync(
            new CreateIndexModel<LogAuditoria>(
                Builders<LogAuditoria>.IndexKeys.Descending(l => l.DataHora),
                new CreateIndexOptions { Name = "ix_data_hora" }),
            cancellationToken: cancellationToken);
    }
}
