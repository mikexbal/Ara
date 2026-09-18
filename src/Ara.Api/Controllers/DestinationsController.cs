using Microsoft.AspNetCore.Mvc;
using Ara.Application.Destinations;

namespace Ara.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinationsController(IDestinationService destinationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DestinationDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await destinationService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DestinationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var destination = await destinationService.GetByIdAsync(id, cancellationToken);
        return destination is null ? NotFound() : Ok(destination);
    }
}
