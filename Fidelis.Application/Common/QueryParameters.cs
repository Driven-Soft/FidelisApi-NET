using System.ComponentModel.DataAnnotations;

namespace Fidelis.Application.Common;

public class QueryParameters
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    [Range(1, int.MaxValue, ErrorMessage = "Page deve ser maior ou igual a 1.")]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "PageSize deve estar entre 1 e 100.")]
    public int PageSize { get; set; } = DefaultPageSize;

    public string? SortBy { get; set; }

    [RegularExpression("^(?i)(asc|desc)$", ErrorMessage = "SortDirection deve ser 'asc' ou 'desc'.")]
    public string? SortDirection { get; set; }

    public bool IsDescending() => string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

    public int Skip() => (Math.Max(Page, 1) - 1) * PageSize;
}
