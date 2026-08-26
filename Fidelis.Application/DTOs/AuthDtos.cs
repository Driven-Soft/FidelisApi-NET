namespace Fidelis.Application.DTOs;

/// <summary>
/// Dados enviados pelo tutor para autenticar (email + senha já cadastrados
/// via POST /api/Tutor).
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// Resposta do login: token JWT a ser enviado no header
/// "Authorization: Bearer {Token}" nas próximas requisições.
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public int TutorId { get; set; }
    public string Nome { get; set; } = string.Empty;
}
