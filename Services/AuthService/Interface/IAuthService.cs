using Lexicon.Services.DataService.Dto;

namespace Lexicon.AuthService.Services.Interface;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequestDto request);

    Task<(AuthResponse Response, Guid SessionId)?> LoginAsync(LoginRequestDto request);

    Task<bool> LogoutAsync(Guid sessionId);
}