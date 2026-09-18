namespace Ara.Application.Destinations;

public interface IDestinationService
{
    Task<IReadOnlyList<DestinationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DestinationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
