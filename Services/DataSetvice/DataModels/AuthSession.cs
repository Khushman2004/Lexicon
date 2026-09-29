namespace Lexicon.Services.DataService.DeleteModels;

public class AuthSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }

    public DateTime? LastUsedAtUtc { get; set; }

    public User User { get; set; } = null!;
}