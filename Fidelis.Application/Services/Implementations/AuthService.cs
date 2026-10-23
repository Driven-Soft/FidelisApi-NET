using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Interfaces;
using Fidelis.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fidelis.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly ITutorRepository _tutorRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ITutorRepository tutorRepository, IOptions<JwtSettings> jwtOptions, ILogger<AuthService> logger)
    {
        _tutorRepository = tutorRepository;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            return null;

        var tutor = _tutorRepository.GetByEmail(request.Email);

        if (tutor is null || tutor.Senha != request.Senha)
        {
            _logger.LogWarning("Falha de autenticação para o email {Email}", request.Email);
            return null;
        }

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

        _logger.LogInformation("Tutor {TutorId} autenticado com sucesso", tutor.Id);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEm = expiraEm,
            TutorId = tutor.Id,
            Nome = tutor.Nome
        };
    }
}
