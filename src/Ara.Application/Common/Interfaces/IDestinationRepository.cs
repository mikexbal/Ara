using Ara.Domain.Entities;

namespace Ara.Application.Common.Interfaces;

public interface IDestinationRepository
{
    Task<IReadOnlyList<Destination>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Destination?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
