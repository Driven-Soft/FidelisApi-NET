using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Oracle.ManagedDataAccess.Client;

namespace Fidelis.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro {StatusCode} ao processar {Method} {Path}", statusCode, httpContext.Request.Method, httpContext.Request.Path);
        else
            _logger.LogWarning("Requisição {Method} {Path} rejeitada com {StatusCode}: {Detail}", httpContext.Request.Method, httpContext.Request.Path, statusCode, exception.Message);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(problem, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json", cancellationToken);

        return true;
    }

    private (int StatusCode, string Title, string? Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Não autorizado", e.Message),
            OracleException e => (StatusCodes.Status503ServiceUnavailable, "Banco de dados relacional indisponível", SafeDetail(e)),
            MongoException e => (StatusCodes.Status503ServiceUnavailable, "Banco de dados NoSQL indisponível", SafeDetail(e)),
            TimeoutException e => (StatusCodes.Status503ServiceUnavailable, "Tempo de resposta excedido", SafeDetail(e)),
            HttpRequestException e => (StatusCodes.Status503ServiceUnavailable, "Serviço de IA indisponível", SafeDetail(e)),
            TaskCanceledException { InnerException: TimeoutException } e => (StatusCodes.Status503ServiceUnavailable, "Tempo de resposta excedido", SafeDetail(e)),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor", _environment.IsDevelopment()
                ? exception.ToString()
                : "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };
    }

    private string SafeDetail(Exception exception) =>
        _environment.IsDevelopment() ? exception.Message : "Serviço temporariamente indisponível. Tente novamente mais tarde.";
}
