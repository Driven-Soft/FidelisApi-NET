using Fidelis.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Fidelis.Infrastructure.Mongo;

public static class MongoMappings
{
    private static readonly object Sync = new();
    private static bool _registered;

    public static void Register()
    {
        lock (Sync)
        {
            if (_registered)
                return;

            BsonClassMap.TryRegisterClassMap<RegistroDiario>(map =>
            {
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(r => r.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                map.MapMember(r => r.PetId).SetElementName("petId");
                map.MapMember(r => r.Titulo).SetElementName("titulo");
                map.MapMember(r => r.Descricao).SetElementName("descricao");
                map.MapMember(r => r.Humor).SetElementName("humor");
                map.MapMember(r => r.Tags).SetElementName("tags");
                map.MapMember(r => r.DataRegistro).SetElementName("dataRegistro");
                map.MapMember(r => r.CriadoEm).SetElementName("criadoEm");
                map.MapMember(r => r.AtualizadoEm).SetElementName("atualizadoEm");
            });

            BsonClassMap.TryRegisterClassMap<LogAuditoria>(map =>
            {
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(l => l.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                map.MapMember(l => l.Metodo).SetElementName("metodo");
                map.MapMember(l => l.Caminho).SetElementName("caminho");
                map.MapMember(l => l.StatusCode).SetElementName("statusCode");
                map.MapMember(l => l.UsuarioId).SetElementName("usuarioId");
                map.MapMember(l => l.UsuarioEmail).SetElementName("usuarioEmail");
                map.MapMember(l => l.DuracaoMs).SetElementName("duracaoMs");
                map.MapMember(l => l.DataHora).SetElementName("dataHora");
            });

            _registered = true;
        }
    }
}
