using Ara.Application.Common.Interfaces;

namespace Ara.Application.Destinations;

public class DestinationService(IDestinationRepository repository) : IDestinationService
{
    public async Task<IReadOnlyList<DestinationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var destinations = await repository.GetAllAsync(cancellationToken);
        return destinations
            .Select(d => new DestinationDto(d.Id, d.Name, d.Island, d.Description, d.Rating, d.ImageUrl))
            .ToList();
    }

    public async Task<DestinationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var destination = await repository.GetByIdAsync(id, cancellationToken);
        return destination is null
            ? null
            : new DestinationDto(destination.Id, destination.Name, destination.Island, destination.Description, destination.Rating, destination.ImageUrl);
    }
}
