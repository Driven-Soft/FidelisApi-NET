using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;

namespace Fidelis.Api.Exceptions;

public class GlobalExceptionHandler(
    IHostEnvironment environment): IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = MapException(environment, exception);
        
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = environment.IsDevelopment() ? detail : string.Empty,
            Instance = httpContext.Request.Path
        };
        
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
    
    
    private static (int StatusCode, string title, string? Detail) MapException(IHostEnvironment environment, Exception exception)
    {
        return exception switch
        {
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Não autorizado", e.Message),
            OracleException e => (StatusCodes.Status500InternalServerError, "Banco de dados indisponível", e.Message),
            MySqlException e=> (StatusCodes.Status500InternalServerError, "Banco de dados indisponível", e.Message),
            _ => MapUnhandled(environment, exception)
        };
    }
    
    private static (int StatusCode, string Title, string? Detail) MapUnhandled(
        IHostEnvironment environment,
        Exception exception)
    {
        if (environment.IsDevelopment())
        {
            return (StatusCodes.Status500InternalServerError, "Erro interno do servidor", exception.ToString());
        }

        return (
            StatusCodes.Status500InternalServerError,
            "Erro interno do servidor",
            "Ocorreu um erro inesperado. Tente novamente mais tarde.");
    }
}