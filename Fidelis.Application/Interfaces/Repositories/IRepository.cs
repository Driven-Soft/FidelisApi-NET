using System.Linq.Expressions;
using Fidelis.Application.Common;

namespace Fidelis.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    IReadOnlyCollection<T> GetAll();
    PagedResult<T> GetPaged(QueryParameters parameters, IEnumerable<Expression<Func<T, bool>>> filters);
    T? GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}
