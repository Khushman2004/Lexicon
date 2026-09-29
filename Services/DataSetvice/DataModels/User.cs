namespace Lexicon.Services.DataService.DeleteModels;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<AuthSession> Sessions { get; set; }
        = new List<AuthSession>();
}