namespace Fidelis.Application.Settings;

/// <summary>
/// Configurações de geração/validação do token JWT, lidas da seção "Jwt"
/// do appsettings.json.
/// </summary>
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 60;
}
