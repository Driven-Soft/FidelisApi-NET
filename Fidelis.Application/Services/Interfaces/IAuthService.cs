using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Valida email/senha de um tutor e, se válidos, retorna um token JWT.
    /// Retorna null quando as credenciais são inválidas.
    /// </summary>
    LoginResponse? Login(LoginRequest request);
}
