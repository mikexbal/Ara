using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Ara.Application.Common.Interfaces;

namespace Ara.Infrastructure.Storage;

public class CloudflareR2ImageStorageService(
    IHttpClientFactory httpClientFactory,
    IOptions<R2Options> options,
    IMemoryCache cache,
    ILogger<CloudflareR2ImageStorageService> logger) : IImageStorageService
{
    private const string HttpClientName = "CloudflareR2";
    private const int MaxWidthPx = 1920;
    private const int JpegQuality = 75;
    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".avif"];
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public Task<IReadOnlyList<string>> ListHeroImageKeysAsync(CancellationToken cancellationToken = default) =>
        ListImageKeysAsync(options.Value.HeroImagePrefix, "hero", cancellationToken);

    public Task<IReadOnlyList<string>> ListSignUpImageKeysAsync(CancellationToken cancellationToken = default) =>
        ListImageKeysAsync(options.Value.SignUpImagePrefix, "sign-up", cancellationToken);

    private async Task<IReadOnlyList<string>> ListImageKeysAsync(string prefix, string label, CancellationToken cancellationToken)
    {
        var r2 = options.Value;

        try
        {
            var client = CreateClient(r2);
            var requestUri =
                $"accounts/{r2.AccountId}/r2/buckets/{r2.BucketName}/objects" +
                $"?prefix={Uri.EscapeDataString(prefix)}&per_page=100";

            var response = await client.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<R2ListObjectsResponse>(JsonOptions, cancellationToken);

            return (payload?.Result ?? [])
                .Select(o => o.Key)
                .Where(key => ImageExtensions.Contains(Path.GetExtension(key).ToLowerInvariant()))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not list {Label} images from R2 bucket {Bucket}", label, r2.BucketName);
            return [];
        }
    }

    public async Task<ImageContent?> GetImageAsync(string key, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"compressed-image:{key}";
        if (cache.TryGetValue(cacheKey, out byte[]? cachedBytes) && cachedBytes is not null)
        {
            return new ImageContent(new MemoryStream(cachedBytes), "image/jpeg");
        }

        var r2 = options.Value;

        try
        {
            var client = CreateClient(r2);
            var requestUri = $"accounts/{r2.AccountId}/r2/buckets/{r2.BucketName}/objects/{Uri.EscapeDataString(key)}";

            var response = await client.GetAsync(requestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var originalBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var compressedBytes = await CompressAsync(originalBytes, cancellationToken);

            cache.Set(cacheKey, compressedBytes, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(6)
            });

            return new ImageContent(new MemoryStream(compressedBytes), "image/jpeg");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not fetch image {Key} from R2 bucket {Bucket}", key, r2.BucketName);
            return null;
        }
    }

    private static async Task<byte[]> CompressAsync(byte[] originalBytes, CancellationToken cancellationToken)
    {
        using var image = Image.Load(originalBytes);

        if (image.Width > MaxWidthPx)
        {
            var newHeight = (int)(image.Height * (MaxWidthPx / (double)image.Width));
            image.Mutate(x => x.Resize(MaxWidthPx, newHeight));
        }

        using var output = new MemoryStream();
        await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = JpegQuality }, cancellationToken);
        return output.ToArray();
    }

    private HttpClient CreateClient(R2Options r2)
    {
        var client = httpClientFactory.CreateClient(HttpClientName);
        client.BaseAddress = new Uri("https://api.cloudflare.com/client/v4/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", r2.ApiToken);
        return client;
    }

    private class R2ListObjectsResponse
    {
        public List<R2ObjectSummary> Result { get; set; } = [];
    }

    private class R2ObjectSummary
    {
        public string Key { get; set; } = string.Empty;
    }
}
