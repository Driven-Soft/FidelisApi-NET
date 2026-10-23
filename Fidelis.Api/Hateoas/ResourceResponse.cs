namespace Fidelis.Api.Hateoas;

public record ResourceResponse<T>(T Data, IReadOnlyList<Link> Links);
