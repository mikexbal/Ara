namespace Ara.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);

    /// <summary>Verifies a password against a hash using a constant-time comparison.</summary>
    bool Verify(string hash, string password);
}
