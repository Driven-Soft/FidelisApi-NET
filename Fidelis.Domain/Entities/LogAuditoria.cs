namespace Fidelis.Domain.Entities;

public class LogAuditoria
{
    public string Id { get; private set; } = string.Empty;

    public string Metodo { get; private set; } = string.Empty;

    public string Caminho { get; private set; } = string.Empty;

    public int StatusCode { get; private set; }

    public string? UsuarioId { get; private set; }

    public string? UsuarioEmail { get; private set; }

    public long DuracaoMs { get; private set; }

    public DateTime DataHora { get; private set; }

    public bool Sucesso => StatusCode is >= 200 and < 400;

    public LogAuditoria(string metodo, string caminho, int statusCode, string? usuarioId, string? usuarioEmail, long duracaoMs)
    {
        if (string.IsNullOrWhiteSpace(metodo))
            throw new ArgumentException("Método HTTP é obrigatório");
        if (string.IsNullOrWhiteSpace(caminho))
            throw new ArgumentException("Caminho é obrigatório");
        if (statusCode is < 100 or > 599)
            throw new ArgumentException("Status code inválido");
        if (duracaoMs < 0)
            throw new ArgumentException("Duração não pode ser negativa");

        Metodo = metodo.Trim().ToUpperInvariant();
        Caminho = caminho.Trim();
        StatusCode = statusCode;
        UsuarioId = usuarioId;
        UsuarioEmail = usuarioEmail;
        DuracaoMs = duracaoMs;
        DataHora = DateTime.UtcNow;
    }

    public override string ToString() => $"{Metodo} {Caminho} -> {StatusCode} ({DuracaoMs}ms)";

    public LogAuditoria() { }
}
