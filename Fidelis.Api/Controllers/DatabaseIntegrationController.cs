using Microsoft.AspNetCore.Mvc;
using Fidelis.Application.DTOs;
using Fidelis.Application.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Fidelis.Api.Controllers;

[ApiController]
[Route("api/database")]
[Produces("application/json")]
public sealed class DatabaseIntegrationController : ControllerBase
{
    private readonly IOraclePackageService _service;

    public DatabaseIntegrationController(IOraclePackageService service)
    {
        _service = service;
    }

    [HttpGet("pets-json")]
    [SwaggerOperation(Summary = "Lista pets usando o package Oracle de JSON")]
    [ProducesResponseType(typeof(OraclePackageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OraclePackageResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PetsJson(
        [FromQuery] int? id,
        CancellationToken cancellationToken)
    {
        var (resultado, mensagem) = await _service.ListarPetsJsonAsync(id, cancellationToken);
        var response = new OraclePackageResponse(resultado, mensagem);

        return IsSuccess(mensagem) ? Ok(response) : UnprocessableEntity(response);
    }

    [HttpGet("resumo-tratamentos")]
    [SwaggerOperation(Summary = "Obtém o resumo de dias de tratamento pelo package Oracle")]
    [ProducesResponseType(typeof(OraclePackageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OraclePackageResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResumoTratamentos(CancellationToken cancellationToken)
    {
        var (resultado, mensagem) = await _service.ResumoTratamentosAsync(cancellationToken);
        var response = new OraclePackageResponse(resultado, mensagem);

        return IsSuccess(mensagem) ? Ok(response) : UnprocessableEntity(response);
    }

    [HttpPost("tutores")]
    [SwaggerOperation(Summary = "Insere um tutor usando o package Oracle de cadastro")]
    [ProducesResponseType(typeof(OraclePackageMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OraclePackageMessageResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> InserirTutor(
        [FromBody] OracleTutorRequest request,
        CancellationToken cancellationToken)
    {
        var mensagem = await _service.InserirTutorAsync(request, cancellationToken);
        var response = new OraclePackageMessageResponse(mensagem);

        return IsSuccess(mensagem) ? Ok(response) : UnprocessableEntity(response);
    }

    private static bool IsSuccess(string? mensagem)
    {
        return string.Equals(mensagem?.Trim(), "OK", StringComparison.OrdinalIgnoreCase);
    }
}