namespace Fidelis.Application.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Tipo { get; set; } = "TUTOR";
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int? TutorId { get; set; }
    public int? VeterinarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
