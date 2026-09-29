using Lexicon.AuthService.DTOs;
using Lexicon.AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lexicon.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        try
        {
            var result =
                await _authService.RegisterAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result =
            await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        Response.Cookies.Append(
            "sessionId",
            result.Value.SessionId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

        return Ok(result.Value.Response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies["sessionId"];

        if (cookie == null ||
            !Guid.TryParse(cookie, out var sessionId))
        {
            return Ok(new
            {
                message = "Already logged out."
            });
        }

        await _authService.LogoutAsync(sessionId);

        Response.Cookies.Delete("sessionId");

        return Ok(new
            {
                message = "Logged out successfully."
            });
    }
}
