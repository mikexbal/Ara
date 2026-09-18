namespace Ara.Application.Auth;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<RegisterResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
