using System.Linq.Expressions;
using Fidelis.Application.Common;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fidelis.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected FidelisContext Context { get; }

    protected DbSet<T> Set => Context.Set<T>();

    public Repository(FidelisContext context)
    {
        Context = context;
    }

    public IReadOnlyCollection<T> GetAll()
    {
        return Set.AsNoTracking().ToList();
    }

    public PagedResult<T> GetPaged(QueryParameters parameters, IEnumerable<Expression<Func<T, bool>>> filters)
    {
        var query = filters.Aggregate(Set.AsNoTracking(), (current, filter) => current.Where(filter));

        var totalItems = query.Count();

        var items = QuerySorter
            .Apply(query, parameters.SortBy, parameters.IsDescending())
            .Skip(parameters.Skip())
            .Take(parameters.PageSize)
            .ToList();

        return new PagedResult<T>(items, parameters.Page, parameters.PageSize, totalItems);
    }

    public T? GetById(int id)
    {
        return Set.Find(id);
    }

    public void Add(T entity)
    {
        Set.Add(entity);
    }

    public void Update(T entity)
    {
        Set.Update(entity);
    }

    public void Delete(T entity)
    {
        Set.Remove(entity);
    }

    public void SaveChanges()
    {
        Context.SaveChanges();
    }
}
