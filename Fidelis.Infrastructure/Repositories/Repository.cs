using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fidelis.Infrastructure.Repositories;

public class Repository<T>(FidelisContext context) : IRepository<T> where T : class
{
    private DbSet<T> Set => context.Set<T>();

    public IReadOnlyCollection<T> GetAll()
    {
        return Set.AsNoTracking().ToList();
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
        context.SaveChanges();
    }
}