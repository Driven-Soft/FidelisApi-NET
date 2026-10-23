using Fidelis.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Fidelis.Api.Hateoas;

public abstract class HateoasController : ControllerBase
{
    protected const string GetAllAction = "GetAll";
    protected const string GetByIdAction = "GetById";
    protected const string UpdateAction = "Update";
    protected const string PatchAction = "Patch";
    protected const string DeleteAction = "Delete";

    protected ResourceResponse<T> ToResource<T>(T data, object id) => new(data, BuildResourceLinks(id));

    protected PagedResponse<T> ToPagedResponse<T>(PagedResult<T> result, Func<T, object> idSelector)
    {
        var items = result.Items
            .Select(item => ToResource(item, idSelector(item)))
            .ToList();

        return new PagedResponse<T>(
            items,
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages,
            BuildPageLinks(result));
    }

    protected CreatedAtActionResult CreatedResource<T>(T data, object id) =>
        CreatedAtAction(GetByIdAction, new { id }, ToResource(data, id));

    private IReadOnlyList<Link> BuildResourceLinks(object id)
    {
        var links = new List<Link>();

        AddLink(links, GetByIdAction, id, "self", HttpMethods.Get);
        AddLink(links, UpdateAction, id, "update", HttpMethods.Put);
        AddLink(links, PatchAction, id, "partial-update", HttpMethods.Patch);
        AddLink(links, DeleteAction, id, "delete", HttpMethods.Delete);
        AddLink(links, GetAllAction, null, "collection", HttpMethods.Get);

        return links;
    }

    private IReadOnlyList<Link> BuildPageLinks<T>(PagedResult<T> result)
    {
        var links = new List<Link>
        {
            new(BuildPageUrl(result.Page, result.PageSize), "self", HttpMethods.Get),
            new(BuildPageUrl(1, result.PageSize), "first", HttpMethods.Get)
        };

        if (result.HasPrevious)
            links.Add(new Link(BuildPageUrl(result.Page - 1, result.PageSize), "previous", HttpMethods.Get));

        if (result.HasNext)
            links.Add(new Link(BuildPageUrl(result.Page + 1, result.PageSize), "next", HttpMethods.Get));

        if (result.TotalPages > 0)
            links.Add(new Link(BuildPageUrl(result.TotalPages, result.PageSize), "last", HttpMethods.Get));

        AddLink(links, "Create", null, "create", HttpMethods.Post);

        return links;
    }

    private void AddLink(List<Link> links, string action, object? id, string rel, string method)
    {
        var values = id is null ? null : new { id };
        var href = Url.Action(action, null, values, Request.Scheme);

        if (!string.IsNullOrWhiteSpace(href))
            links.Add(new Link(href, rel, method));
    }

    private string BuildPageUrl(int page, int pageSize)
    {
        var parameters = Request.Query
            .Where(q => !q.Key.Equals("page", StringComparison.OrdinalIgnoreCase)
                        && !q.Key.Equals("pageSize", StringComparison.OrdinalIgnoreCase))
            .SelectMany(q => q.Value.Select(value => new KeyValuePair<string, string?>(q.Key, value)))
            .Append(new KeyValuePair<string, string?>("page", page.ToString()))
            .Append(new KeyValuePair<string, string?>("pageSize", pageSize.ToString()));

        return $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}{QueryString.Create(parameters)}";
    }
}
