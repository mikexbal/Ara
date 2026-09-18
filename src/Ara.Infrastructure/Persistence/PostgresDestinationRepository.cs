using Microsoft.EntityFrameworkCore;
using Ara.Application.Common.Interfaces;
using Ara.Domain.Entities;

namespace Ara.Infrastructure.Persistence;

public class PostgresDestinationRepository(AraDbContext dbContext) : IDestinationRepository
{
    public async Task<IReadOnlyList<Destination>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Destinations.AsNoTracking().OrderBy(d => d.Id).ToListAsync(cancellationToken);

    public Task<Destination?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => dbContext.Destinations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
}
