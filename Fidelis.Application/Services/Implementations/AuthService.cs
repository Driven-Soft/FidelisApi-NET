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
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ITutorRepository tutorRepository,
        IVeterinarioRepository veterinarioRepository,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthService> logger)
    {
        _tutorRepository = tutorRepository;
        _veterinarioRepository = veterinarioRepository;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        var tipo = request.Tipo.Trim().ToUpperInvariant();
        if (tipo != "TUTOR" && tipo != "VETERINARIO")
            throw new ArgumentException("Tipo deve ser TUTOR ou VETERINARIO.");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            return null;

        var response = tipo == "TUTOR" ? LoginTutor(request) : LoginVeterinario(request);

        if (response is null)
            _logger.LogWarning("Falha de autenticação para o email {Email}", request.Email);

        return response;
    }

    private LoginResponse? LoginTutor(LoginRequest request)
    {
        var tutor = _tutorRepository.GetByEmail(request.Email);

        if (tutor is null || tutor.Senha != request.Senha)
            return null;

        var response = GerarLogin("TUTOR", tutor.Id, tutor.Email, tutor.Nome);
        response.TutorId = tutor.Id;

        _logger.LogInformation("Tutor {TutorId} autenticado com sucesso", tutor.Id);

        return response;
    }

    private LoginResponse? LoginVeterinario(LoginRequest request)
    {
        var veterinario = _veterinarioRepository.GetByEmail(request.Email);

        if (veterinario is null || veterinario.Senha != request.Senha)
            return null;

        var response = GerarLogin("VETERINARIO", veterinario.Id, veterinario.Email, veterinario.Nome);
        response.VeterinarioId = veterinario.Id;

        _logger.LogInformation("Veterinário {VeterinarioId} autenticado com sucesso", veterinario.Id);

        return response;
    }

    private LoginResponse GerarLogin(string tipo, int id, string email, string nome)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, id.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Name, nome),
            new(ClaimTypes.Role, tipo),
            new("tipo", tipo),
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
            Tipo = tipo,
            Nome = nome,
            Email = email
        };
    }
}
