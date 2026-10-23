using Fidelis.Application.Common;
using Fidelis.Domain.Entities;

namespace Fidelis.Application.DTOs;

public class AuditoriaResponse
{
    public string Id { get; }
    public string Metodo { get; }
    public string Caminho { get; }
    public int StatusCode { get; }
    public bool Sucesso { get; }
    public string? UsuarioId { get; }
    public string? UsuarioEmail { get; }
    public long DuracaoMs { get; }
    public DateTime DataHora { get; }

    public AuditoriaResponse(LogAuditoria log)
    {
        Id = log.Id;
        Metodo = log.Metodo;
        Caminho = log.Caminho;
        StatusCode = log.StatusCode;
        Sucesso = log.Sucesso;
        UsuarioId = log.UsuarioId;
        UsuarioEmail = log.UsuarioEmail;
        DuracaoMs = log.DuracaoMs;
        DataHora = log.DataHora;
    }
}

public class AuditoriaQueryParameters : QueryParameters
{
    public string? Metodo { get; set; }

    public string? Caminho { get; set; }

    public int? StatusCode { get; set; }

    public string? UsuarioId { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }
}
