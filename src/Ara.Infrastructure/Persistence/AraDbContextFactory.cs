using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ara.Infrastructure.Persistence;

public class AraDbContextFactory : IDesignTimeDbContextFactory<AraDbContext>
{
    public AraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AraDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ara;Username=postgres;Password=postgres");
        return new AraDbContext(optionsBuilder.Options);
    }
}
