using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IOraclePackageService
{
    Task<(string? Resultado, string? Mensagem)> ListarPetsJsonAsync(
        int? id,
        CancellationToken cancellationToken);

    Task<(string? Resultado, string? Mensagem)> ResumoTratamentosAsync(
        CancellationToken cancellationToken);

    Task<string?> InserirTutorAsync(
        OracleTutorRequest request,
        CancellationToken cancellationToken);
}
