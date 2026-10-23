using Fidelis.Application.DTOs;

namespace Fidelis.Application.Services.Interfaces;

public interface IAuthService
{
    LoginResponse? Login(LoginRequest request);
}
