using Lexicon.AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace Lexicon.Services.DataService.DbContext;

public class DbContext 
{
    public DbContext(
        DbContextOptions<LexiconDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<AuthSession> AuthSessions => Set<AuthSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Username)
                  .IsUnique();

            entity.HasIndex(x => x.Email)
                  .IsUnique();

            entity.Property(x => x.Username)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(x => x.Email)
                  .HasMaxLength(320)
                  .IsRequired();

            entity.Property(x => x.PasswordHash)
                  .HasMaxLength(500)
                  .IsRequired();
        });

        modelBuilder.Entity<AuthSession>(entity =>
        {
            entity.ToTable("AuthSessions");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.UserId);

            entity.HasOne(x => x.User)
                  .WithMany(x => x.Sessions)
                  .HasForeignKey(x => x.UserId);
        });
    }
}