namespace Fidelis.Application.Common;

public static class TextFilter
{
    public static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
}
