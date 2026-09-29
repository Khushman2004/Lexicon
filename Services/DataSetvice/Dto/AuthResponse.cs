namespace Lexicon.Services.DataService.Dto;

public class AuthResponse
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Message { get; set; } = null!;
}