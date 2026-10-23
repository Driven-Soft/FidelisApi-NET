using System.Collections.Concurrent;
using Fidelis.Application.Common;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Domain.Entities;
using Fidelis.Infrastructure.Repositories;
using MongoDB.Bson;

namespace Fidelis.IntegrationTests.Fakes;

public class InMemoryRegistroDiarioRepository : IRegistroDiarioRepository
{
    private readonly ConcurrentDictionary<string, RegistroDiario> _store = new();

    public PagedResult<RegistroDiario> GetPaged(RegistroDiarioQueryParameters parameters)
    {
        var query = _store.Values.AsQueryable();

        if (parameters.PetId.HasValue)
            query = query.Where(r => r.PetId == parameters.PetId.Value);

        if (!string.IsNullOrWhiteSpace(parameters.Humor))
            query = query.Where(r => r.Humor.Equals(parameters.Humor.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(parameters.Tag))
            query = query.Where(r => r.Tags.Contains(parameters.Tag.Trim().ToLowerInvariant()));

        if (!string.IsNullOrWhiteSpace(parameters.Texto))
            query = query.Where(r => r.Titulo.Contains(parameters.Texto, StringComparison.OrdinalIgnoreCase)
                                     || r.Descricao.Contains(parameters.Texto, StringComparison.OrdinalIgnoreCase));

        var sortBy = string.IsNullOrWhiteSpace(parameters.SortBy) ? nameof(RegistroDiario.DataRegistro) : parameters.SortBy;
        var descending = string.IsNullOrWhiteSpace(parameters.SortBy) || parameters.IsDescending();
        var ordered = QuerySorter.Apply(query, sortBy, descending).ToList();

        var items = ordered.Skip(parameters.Skip()).Take(parameters.PageSize).ToList();
        return new PagedResult<RegistroDiario>(items, parameters.Page, parameters.PageSize, ordered.Count);
    }

    public RegistroDiario? GetById(string id) => _store.GetValueOrDefault(id);

    public void Add(RegistroDiario registro)
    {
        typeof(RegistroDiario).GetProperty(nameof(RegistroDiario.Id))!.SetValue(registro, ObjectId.GenerateNewId().ToString());
        _store[registro.Id] = registro;
    }

    public bool Replace(RegistroDiario registro)
    {
        if (!_store.ContainsKey(registro.Id))
            return false;

        _store[registro.Id] = registro;
        return true;
    }

    public bool Delete(string id) => _store.TryRemove(id, out _);
}
