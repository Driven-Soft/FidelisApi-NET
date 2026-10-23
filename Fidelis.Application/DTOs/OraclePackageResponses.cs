namespace Fidelis.Application.DTOs;

/// <summary>Resultado retornado por uma procedure Oracle que disponibiliza dados.</summary>
public sealed record OraclePackageResponse(string? Resultado, string? Mensagem);

/// <summary>Mensagem retornada por uma procedure Oracle sem conjunto de dados.</summary>
public sealed record OraclePackageMessageResponse(string? Mensagem);
