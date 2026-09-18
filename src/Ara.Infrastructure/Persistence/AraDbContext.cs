using Microsoft.EntityFrameworkCore;
using Ara.Domain.Entities;

namespace Ara.Infrastructure.Persistence;

public class AraDbContext(DbContextOptions<AraDbContext> options) : DbContext(options)
{
    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Destination>(entity =>
        {
            entity.ToTable("destinations");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(200);
            entity.Property(d => d.Island).IsRequired().HasMaxLength(200);
            entity.Property(d => d.Description).IsRequired();
            entity.Property(d => d.Rating).IsRequired();

            entity.HasData(
                new Destination { Id = 1, Name = "Bora Bora", Island = "Society Islands", Description = "Turquoise lagoon and overwater bungalows.", Rating = 4.9 },
                new Destination { Id = 2, Name = "Tahiti", Island = "Society Islands", Description = "Black sand beaches and the gateway to French Polynesia.", Rating = 4.6 },
                new Destination { Id = 3, Name = "Moorea", Island = "Society Islands", Description = "Jagged volcanic peaks overlooking a coral reef.", Rating = 4.8 },
                new Destination { Id = 4, Name = "Rangiroa", Island = "Tuamotu Archipelago", Description = "One of the largest atolls in the world, prized for diving.", Rating = 4.7 }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users", t => t.HasCheckConstraint("CK_users_email_contains_at", "\"Email\" LIKE '%@%'"));
            entity.HasKey(u => u.Id);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(25);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(25);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(254);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.EmailConfirmed).IsRequired().HasDefaultValue(false);
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.UpdatedAt).IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}
