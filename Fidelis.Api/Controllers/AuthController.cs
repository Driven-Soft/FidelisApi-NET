using Fidelis.Application.DTOs;
using Fidelis.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Fidelis.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Autenticação de tutores e veterinários via JWT.")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Autentica um tutor ou veterinário e retorna o token JWT",
        Description = "Informe tipo TUTOR (padrão quando omitido) ou VETERINARIO, com o email e a senha cadastrados em POST /api/Tutor ou POST /api/Veterinario. A resposta traz tutorId ou veterinarioId conforme o tipo. Envie o token retornado no header Authorization: Bearer {token}.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var result = _authService.Login(request);
        return result is null ? Unauthorized(new { message = "Email ou senha inválidos." }) : Ok(result);
    }
}
