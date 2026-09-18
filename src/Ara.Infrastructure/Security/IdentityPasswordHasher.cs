using Microsoft.AspNetCore.Identity;
using Ara.Application.Common.Interfaces;
using Ara.Domain.Entities;

namespace Ara.Infrastructure.Security;

/// <summary>
/// Wraps ASP.NET Core Identity's PasswordHasher — PBKDF2-HMAC-SHA256 with 100,000 iterations
/// and a per-password random salt (NIST SP 800-63B compliant). Deliberately not a hand-rolled
/// hashing scheme: password hashing is not something to reimplement.
/// </summary>
public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(default!, password);

    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(default!, hash, password) != PasswordVerificationResult.Failed;
}
