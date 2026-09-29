using Lexicon.AuthService.Data;
using Lexicon.AuthService.DTOs;
using Lexicon.AuthService.Models;
using Lexicon.AuthService.Security;
using Lexicon.AuthService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Lexicon.AuthService.Interface;
using Lexicon.Services.DataService.DbContext;

namespace Lexicon.AuthService.Implementation;

public class AuthService : IAuthService
{
    private readonly DbContext _db;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        DbContext db,
        PasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var existingUser = await _db.Users
            .FirstOrDefaultAsync(x =>
                x.Username == request.Username ||
                x.Email == request.Email);

        if (existingUser != null)
        {
            throw new Exception("Username or email already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Message = "Registration successful."
        };
    }

    public async Task<(AuthResponse Response, Guid SessionId)?> LoginAsync(
        LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x =>
                x.Username == request.UsernameOrEmail ||
                x.Email == request.UsernameOrEmail);

        if (user == null || !user.IsActive)
            return null;

        var passwordValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
            return null;

        var session = new AuthSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            LastUsedAtUtc = DateTime.UtcNow
        };

        _db.AuthSessions.Add(session);
        await _db.SaveChangesAsync();

        return new
        (
            new AuthResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Message = "Login successful."
            },
            session.Id
        );
    }

    public async Task<bool> LogoutAsync(Guid sessionId)
    {
        var session = await _db.AuthSessions
            .FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session == null)
            return false;

        session.RevokedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return true;
    }
}