using Microsoft.Extensions.Caching.Memory;
using Ara.Application.Common.Interfaces;

namespace Ara.Infrastructure.Security;

public class MemoryVerificationCodeStore(IMemoryCache cache) : IVerificationCodeStore
{
    public void Set(string email, string code, TimeSpan timeToLive)
        => cache.Set(CacheKey(email), code, timeToLive);

    public bool TryGet(string email, out string? code)
        => cache.TryGetValue(CacheKey(email), out code);

    public void Remove(string email) => cache.Remove(CacheKey(email));

    private static string CacheKey(string email) => $"email-verification-code:{email}";
}
