using Microsoft.AspNetCore.Mvc;
using Ara.Application.Common.Interfaces;

namespace Ara.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController(IImageStorageService imageStorageService) : ControllerBase
{
    [HttpGet("hero")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetHeroImages(CancellationToken cancellationToken)
    {
        var keys = await imageStorageService.ListHeroImageKeysAsync(cancellationToken);
        var urls = keys.Select(key => Url.Action(nameof(GetHeroImageFile), new { key })!).ToList();
        return Ok(urls);
    }

    [HttpGet("hero/file")]
    public async Task<IActionResult> GetHeroImageFile([FromQuery] string key, CancellationToken cancellationToken)
    {
        var image = await imageStorageService.GetImageAsync(key, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "public, max-age=3600";
        return File(image.Content, image.ContentType);
    }
}
