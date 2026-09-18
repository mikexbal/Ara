namespace Ara.Application.Common.Interfaces;

public interface IVerificationCodeStore
{
    void Set(string email, string code, TimeSpan timeToLive);
    bool TryGet(string email, out string? code);
    void Remove(string email);
}
