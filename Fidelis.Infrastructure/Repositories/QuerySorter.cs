using System.Linq.Expressions;
using System.Reflection;

namespace Fidelis.Infrastructure.Repositories;

public static class QuerySorter
{
    private const string DefaultSortProperty = "Id";

    public static IQueryable<T> Apply<T>(IQueryable<T> query, string? sortBy, bool descending)
    {
        var property = ResolveProperty<T>(sortBy);
        var parameter = Expression.Parameter(typeof(T), "entity");
        var body = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(body, parameter);
        var methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

        var call = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.PropertyType],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(call);
    }

    public static PropertyInfo ResolveProperty<T>(string? sortBy)
    {
        var name = string.IsNullOrWhiteSpace(sortBy) ? DefaultSortProperty : sortBy.Trim();
        var property = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (property is null || !IsSortable(property.PropertyType))
            throw new ArgumentException($"Campo de ordenação inválido: '{name}'.");

        return property;
    }

    private static bool IsSortable(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        return underlying.IsPrimitive
               || underlying.IsEnum
               || underlying == typeof(string)
               || underlying == typeof(decimal)
               || underlying == typeof(DateTime)
               || underlying == typeof(DateTimeOffset)
               || underlying == typeof(Guid);
    }
}
