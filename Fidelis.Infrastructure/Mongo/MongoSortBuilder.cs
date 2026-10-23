using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Fidelis.Infrastructure.Mongo;

public static class MongoSortBuilder
{
    public static SortDefinition<T> Build<T>(string? sortBy, bool descending, string defaultProperty)
    {
        var propertyName = string.IsNullOrWhiteSpace(sortBy) ? defaultProperty : sortBy.Trim();

        var memberMap = BsonClassMap.LookupClassMap(typeof(T))
            .AllMemberMaps
            .FirstOrDefault(m => m.MemberName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        if (memberMap is null)
            throw new ArgumentException($"Campo de ordenação inválido: '{propertyName}'.");

        var field = new StringFieldDefinition<T>(memberMap.ElementName);

        return descending
            ? Builders<T>.Sort.Descending(field)
            : Builders<T>.Sort.Ascending(field);
    }
}
