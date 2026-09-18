namespace Ara.Application.Common.Interfaces;

public record ImageContent(Stream Content, string ContentType);

public interface IImageStorageService
{
    Task<IReadOnlyList<string>> ListHeroImageKeysAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> ListSignUpImageKeysAsync(CancellationToken cancellationToken = default);
    Task<ImageContent?> GetImageAsync(string key, CancellationToken cancellationToken = default);
}
