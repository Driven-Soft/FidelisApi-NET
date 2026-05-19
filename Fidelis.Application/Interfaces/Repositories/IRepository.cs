namespace Fidelis.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    IReadOnlyCollection<T> GetAll();
    T? GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}