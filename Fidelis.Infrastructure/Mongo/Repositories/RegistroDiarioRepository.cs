using System.Text.RegularExpressions;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fidelis.Infrastructure.Mongo.Repositories;

public class RegistroDiarioRepository : IRegistroDiarioRepository
{
    private readonly IMongoCollection<RegistroDiario> _collection;

    public RegistroDiarioRepository(MongoDbContext context)
    {
        _collection = context.RegistrosDiario;
    }

    public PagedResult<RegistroDiario> GetPaged(RegistroDiarioQueryParameters parameters)
    {
        var filter = BuildFilter(parameters);
        var sortBy = string.IsNullOrWhiteSpace(parameters.SortBy) ? nameof(RegistroDiario.DataRegistro) : parameters.SortBy;
        var descending = string.IsNullOrWhiteSpace(parameters.SortBy) || parameters.IsDescending();
        var sort = MongoSortBuilder.Build<RegistroDiario>(sortBy, descending, nameof(RegistroDiario.DataRegistro));

        var totalItems = (int)_collection.CountDocuments(filter);

        var items = _collection
            .Find(filter)
            .Sort(sort)
            .Skip(parameters.Skip())
            .Limit(parameters.PageSize)
            .ToList();

        return new PagedResult<RegistroDiario>(items, parameters.Page, parameters.PageSize, totalItems);
    }

    public RegistroDiario? GetById(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return _collection.Find(r => r.Id == id).FirstOrDefault();
    }

    public void Add(RegistroDiario registro)
    {
        _collection.InsertOne(registro);
    }

    public bool Replace(RegistroDiario registro)
    {
        if (!ObjectId.TryParse(registro.Id, out _))
            return false;

        var result = _collection.ReplaceOne(r => r.Id == registro.Id, registro);
        return result.MatchedCount > 0;
    }

    public bool Delete(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var result = _collection.DeleteOne(r => r.Id == id);
        return result.DeletedCount > 0;
    }

    private static FilterDefinition<RegistroDiario> BuildFilter(RegistroDiarioQueryParameters parameters)
    {
        var builder = Builders<RegistroDiario>.Filter;
        var filters = new List<FilterDefinition<RegistroDiario>>();

        if (parameters.PetId.HasValue)
            filters.Add(builder.Eq(r => r.PetId, parameters.PetId.Value));

        if (!string.IsNullOrWhiteSpace(parameters.Humor))
            filters.Add(builder.Regex(r => r.Humor, new BsonRegularExpression($"^{Regex.Escape(parameters.Humor.Trim())}$", "i")));

        if (!string.IsNullOrWhiteSpace(parameters.Tag))
            filters.Add(builder.AnyEq(r => r.Tags, parameters.Tag.Trim().ToLowerInvariant()));

        if (!string.IsNullOrWhiteSpace(parameters.Texto))
        {
            var regex = new BsonRegularExpression(Regex.Escape(parameters.Texto.Trim()), "i");
            filters.Add(builder.Or(
                builder.Regex(r => r.Titulo, regex),
                builder.Regex(r => r.Descricao, regex)));
        }

        if (parameters.DataInicio.HasValue)
            filters.Add(builder.Gte(r => r.DataRegistro, parameters.DataInicio.Value));

        if (parameters.DataFim.HasValue)
            filters.Add(builder.Lte(r => r.DataRegistro, parameters.DataFim.Value));

        return filters.Count == 0 ? builder.Empty : builder.And(filters);
    }
}
