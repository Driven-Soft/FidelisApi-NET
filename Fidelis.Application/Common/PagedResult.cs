namespace Fidelis.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;

    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
    }

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> selector) =>
        new(Items.Select(selector).ToList(), Page, PageSize, TotalItems);
}
