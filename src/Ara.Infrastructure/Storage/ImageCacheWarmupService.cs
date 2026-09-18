using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ara.Application.Common.Interfaces;

namespace Ara.Infrastructure.Storage;

/// <summary>
/// Pre-fetches and compresses every known R2 image on startup so the first real
/// visitor never pays the cold-cache cost (multi-second R2 download + resize).
/// </summary>
public class ImageCacheWarmupService(
    IImageStorageService imageStorageService,
    ILogger<ImageCacheWarmupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var heroKeys = await imageStorageService.ListHeroImageKeysAsync(stoppingToken);
            var signUpKeys = await imageStorageService.ListSignUpImageKeysAsync(stoppingToken);
            var logInKeys = await imageStorageService.ListLogInImageKeysAsync(stoppingToken);
            var keys = heroKeys.Concat(signUpKeys).Concat(logInKeys).ToList();

            foreach (var key in keys)
            {
                await imageStorageService.GetImageAsync(key, stoppingToken);
            }

            logger.LogInformation("Warmed image cache for {Count} keys", keys.Count);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Image cache warmup failed; images will warm lazily on first request instead");
        }
    }
}
