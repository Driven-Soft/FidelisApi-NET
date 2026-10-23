using System.Linq.Expressions;

namespace Fidelis.Application.Common;

public class FilterBuilder<T>
{
    private readonly List<Expression<Func<T, bool>>> _filters = [];

    public FilterBuilder<T> AddIf(bool condition, Expression<Func<T, bool>> filter)
    {
        if (condition)
            _filters.Add(filter);

        return this;
    }

    public IReadOnlyList<Expression<Func<T, bool>>> Build() => _filters;
}
