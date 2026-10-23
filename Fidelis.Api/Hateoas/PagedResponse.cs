namespace Fidelis.Api.Hateoas;

public record PagedResponse<T>(
    IReadOnlyList<ResourceResponse<T>> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    IReadOnlyList<Link> Links);
