using System.Collections.Concurrent;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using MongoDB.Bson;

namespace Fidelis.IntegrationTests.Fakes;

public class InMemoryAuditoriaRepository : IAuditoriaRepository
{
    private readonly ConcurrentDictionary<string, LogAuditoria> _store = new();

    public PagedResult<LogAuditoria> GetPaged(AuditoriaQueryParameters parameters)
    {
        var query = _store.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(parameters.Metodo))
            query = query.Where(l => l.Metodo.Equals(parameters.Metodo, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(parameters.Caminho))
            query = query.Where(l => l.Caminho.Contains(parameters.Caminho, StringComparison.OrdinalIgnoreCase));

        if (parameters.StatusCode.HasValue)
            query = query.Where(l => l.StatusCode == parameters.StatusCode.Value);

        var ordered = query.OrderByDescending(l => l.DataHora).ToList();
        var items = ordered.Skip(parameters.Skip()).Take(parameters.PageSize).ToList();

        return new PagedResult<LogAuditoria>(items, parameters.Page, parameters.PageSize, ordered.Count);
    }

    public LogAuditoria? GetById(string id) => _store.GetValueOrDefault(id);

    public void Add(LogAuditoria log)
    {
        typeof(LogAuditoria).GetProperty(nameof(LogAuditoria.Id))!.SetValue(log, ObjectId.GenerateNewId().ToString());
        _store[log.Id] = log;
    }
}
