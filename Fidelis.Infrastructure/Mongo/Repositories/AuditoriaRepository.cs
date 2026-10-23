using System.Text.RegularExpressions;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Fidelis.Infrastructure.Mongo.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly IMongoCollection<LogAuditoria> _collection;

    public AuditoriaRepository(MongoDbContext context)
    {
        _collection = context.LogsAuditoria;
    }

    public PagedResult<LogAuditoria> GetPaged(AuditoriaQueryParameters parameters)
    {
        var filter = BuildFilter(parameters);
        var sortBy = string.IsNullOrWhiteSpace(parameters.SortBy) ? nameof(LogAuditoria.DataHora) : parameters.SortBy;
        var descending = string.IsNullOrWhiteSpace(parameters.SortBy) || parameters.IsDescending();
        var sort = MongoSortBuilder.Build<LogAuditoria>(sortBy, descending, nameof(LogAuditoria.DataHora));

        var totalItems = (int)_collection.CountDocuments(filter);

        var items = _collection
            .Find(filter)
            .Sort(sort)
            .Skip(parameters.Skip())
            .Limit(parameters.PageSize)
            .ToList();

        return new PagedResult<LogAuditoria>(items, parameters.Page, parameters.PageSize, totalItems);
    }

    public LogAuditoria? GetById(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return _collection.Find(l => l.Id == id).FirstOrDefault();
    }

    public void Add(LogAuditoria log)
    {
        _collection.InsertOne(log);
    }

    private static FilterDefinition<LogAuditoria> BuildFilter(AuditoriaQueryParameters parameters)
    {
        var builder = Builders<LogAuditoria>.Filter;
        var filters = new List<FilterDefinition<LogAuditoria>>();

        if (!string.IsNullOrWhiteSpace(parameters.Metodo))
            filters.Add(builder.Eq(l => l.Metodo, parameters.Metodo.Trim().ToUpperInvariant()));

        if (!string.IsNullOrWhiteSpace(parameters.Caminho))
            filters.Add(builder.Regex(l => l.Caminho, new BsonRegularExpression(Regex.Escape(parameters.Caminho.Trim()), "i")));

        if (parameters.StatusCode.HasValue)
            filters.Add(builder.Eq(l => l.StatusCode, parameters.StatusCode.Value));

        if (!string.IsNullOrWhiteSpace(parameters.UsuarioId))
            filters.Add(builder.Eq(l => l.UsuarioId, parameters.UsuarioId.Trim()));

        if (parameters.DataInicio.HasValue)
            filters.Add(builder.Gte(l => l.DataHora, parameters.DataInicio.Value));

        if (parameters.DataFim.HasValue)
            filters.Add(builder.Lte(l => l.DataHora, parameters.DataFim.Value));

        return filters.Count == 0 ? builder.Empty : builder.And(filters);
    }
}
