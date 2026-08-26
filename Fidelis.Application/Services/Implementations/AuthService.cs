using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Fidelis.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fidelis.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ITutorRepository _tutorRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(ITutorRepository tutorRepository, IOptions<JwtSettings> jwtOptions)
    {
        _tutorRepository = tutorRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        var tutor = _tutorRepository.GetAll()
            .Where(t => t.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.DataCriacao)
            .FirstOrDefault();

        // Observação: comparação em texto puro porque a senha do Tutor hoje é
        // persistida sem hash. Recomendado migrar para BCrypt/PBKDF2 depois;
        // isso ficou fora do escopo mínimo para fechar o requisito de auth.
        if (tutor is null || tutor.Senha != request.Senha)
            return null;

        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, tutor.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, tutor.Email),
            new(ClaimTypes.Name, tutor.Nome),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm,
            TutorId = tutor.Id,
            Nome = tutor.Nome
        };
    }
}
