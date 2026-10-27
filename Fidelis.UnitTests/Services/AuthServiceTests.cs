using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Application.Settings;
using Fidelis.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<ITutorRepository> _repositoryMock;
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _repositoryMock = new Mock<ITutorRepository>();
        _veterinarioRepositoryMock = new Mock<IVeterinarioRepository>();
        var settings = Options.Create(new JwtSettings
        {
            Key = "chave-de-teste-com-pelo-menos-32-caracteres!!",
            Issuer = "FidelisApi",
            Audience = "FidelisApiUsers",
            ExpiresInMinutes = 30
        });
        _sut = new AuthService(_repositoryMock.Object, _veterinarioRepositoryMock.Object, settings, NullLogger<AuthService>.Instance);
    }

    private static Tutor NovoTutor() => new("12345678900", "Maria", "maria@example.com", "123456", "11999999999", "Rua A");

    private static Veterinario NovoVeterinario() => new("SP-12345", "Dra. Ana", "ana@clinica.com", "123456", "Clínica geral", 1);

    [Fact]
    public void Login_CredenciaisValidas_RetornaTokenComClaimsDoTutor()
    {
        _repositoryMock.Setup(r => r.GetByEmail("maria@example.com")).Returns(NovoTutor());

        var result = _sut.Login(new LoginRequest { Email = "maria@example.com", Senha = "123456" });

        Assert.NotNull(result);
        Assert.Equal("Maria", result!.Nome);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Equal("FidelisApi", token.Issuer);
        Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "maria@example.com");
        Assert.True(result.ExpiraEm > DateTime.UtcNow);
    }

    [Fact]
    public void Login_SemTipo_AutenticaComoTutor()
    {
        _repositoryMock.Setup(r => r.GetByEmail("maria@example.com")).Returns(NovoTutor());

        var result = _sut.Login(new LoginRequest { Email = "maria@example.com", Senha = "123456" });

        Assert.Equal("TUTOR", result!.Tipo);
        Assert.NotNull(result.TutorId);
        Assert.Null(result.VeterinarioId);
        Assert.Equal("maria@example.com", result.Email);
        _veterinarioRepositoryMock.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Login_SenhaIncorreta_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(NovoTutor());

        var result = _sut.Login(new LoginRequest { Email = "maria@example.com", Senha = "errada" });

        Assert.Null(result);
    }

    [Fact]
    public void Login_EmailInexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Tutor?)null);

        var result = _sut.Login(new LoginRequest { Email = "ninguem@example.com", Senha = "123456" });

        Assert.Null(result);
    }

    [Theory]
    [InlineData("", "123456")]
    [InlineData("maria@example.com", "")]
    public void Login_CamposVazios_RetornaNullSemConsultarRepositorio(string email, string senha)
    {
        var result = _sut.Login(new LoginRequest { Email = email, Senha = senha });

        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData("VETERINARIO")]
    [InlineData("veterinario")]
    public void Login_VeterinarioComCredenciaisValidas_RetornaSessaoDoVeterinario(string tipo)
    {
        _veterinarioRepositoryMock.Setup(r => r.GetByEmail("ana@clinica.com")).Returns(NovoVeterinario());

        var result = _sut.Login(new LoginRequest { Email = "ana@clinica.com", Senha = "123456", Tipo = tipo });

        Assert.Equal("VETERINARIO", result!.Tipo);
        Assert.NotNull(result.VeterinarioId);
        Assert.Null(result.TutorId);
        Assert.Equal("Dra. Ana", result.Nome);
        Assert.Equal("ana@clinica.com", result.Email);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "VETERINARIO");
        Assert.Contains(token.Claims, c => c.Type == "tipo" && c.Value == "VETERINARIO");
    }

    [Fact]
    public void Login_VeterinarioComSenhaIncorreta_RetornaNull()
    {
        _veterinarioRepositoryMock.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns(NovoVeterinario());

        var result = _sut.Login(new LoginRequest { Email = "ana@clinica.com", Senha = "errada", Tipo = "VETERINARIO" });

        Assert.Null(result);
    }

    [Fact]
    public void Login_EmailDeTutorNoPortalDoVeterinario_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetByEmail("maria@example.com")).Returns(NovoTutor());

        var result = _sut.Login(new LoginRequest { Email = "maria@example.com", Senha = "123456", Tipo = "VETERINARIO" });

        Assert.Null(result);
        _repositoryMock.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Login_TipoInvalido_LancaArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            _sut.Login(new LoginRequest { Email = "maria@example.com", Senha = "123456", Tipo = "ADMIN" }));
    }
}
