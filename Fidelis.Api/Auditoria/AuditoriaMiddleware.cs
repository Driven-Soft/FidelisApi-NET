using System.Diagnostics;
using System.Security.Claims;
using Fidelis.Domain.Entities;

namespace Fidelis.Api.Auditoria;

public class AuditoriaMiddleware
{
    private static readonly HashSet<string> MetodosAuditados = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Patch,
        HttpMethods.Delete
    };

    private readonly RequestDelegate _next;
    private readonly IAuditoriaQueue _queue;

    public AuditoriaMiddleware(RequestDelegate next, IAuditoriaQueue queue)
    {
        _next = next;
        _queue = queue;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!DeveAuditar(context.Request))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var statusCode = StatusCodes.Status500InternalServerError;

        try
        {
            await _next(context);
            statusCode = context.Response.StatusCode;
        }
        finally
        {
            stopwatch.Stop();

            var usuario = context.User;
            var usuarioId = usuario.FindFirstValue(ClaimTypes.NameIdentifier) ?? usuario.FindFirstValue("sub");
            var usuarioEmail = usuario.FindFirstValue(ClaimTypes.Email) ?? usuario.FindFirstValue("email");

            _queue.Enqueue(new LogAuditoria(
                context.Request.Method,
                context.Request.Path.Value ?? "/",
                statusCode,
                usuarioId,
                usuarioEmail,
                stopwatch.ElapsedMilliseconds));
        }
    }

    private static bool DeveAuditar(HttpRequest request) =>
        MetodosAuditados.Contains(request.Method)
        && request.Path.StartsWithSegments("/api")
        && !request.Path.StartsWithSegments("/api/Auth");
}
