using System.ComponentModel.DataAnnotations;
using Ara.Application.Common.Validation;

namespace Ara.Application.Auth;

public record RegisterRequest(
    [Required, MaxLength(25)] string FirstName,
    [Required, MaxLength(25)] string LastName,
    [Required, EmailAddress] string Email,
    [Required, PasswordComplexity] string Password
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record VerifyEmailRequest(
    [Required, EmailAddress] string Email,
    [Required, StringLength(6, MinimumLength = 6)] string Code
);

public record ResendVerificationRequest(
    [Required, EmailAddress] string Email
);

public record UserDto(Guid Id, string FirstName, string LastName, string Email, bool EmailConfirmed, DateTime CreatedAt);

public record AuthResponse(string Token, UserDto User);

public record AuthResult(bool Succeeded, string? Error, AuthResponse? Response)
{
    public static AuthResult Success(AuthResponse response) => new(true, null, response);
    public static AuthResult Failure(string error) => new(false, error, null);
}

public record RegistrationPendingResponse(string Email, int ExpiresInSeconds);

public record RegisterResult(bool Succeeded, string? Error, RegistrationPendingResponse? Pending)
{
    public static RegisterResult Success(RegistrationPendingResponse pending) => new(true, null, pending);
    public static RegisterResult Failure(string error) => new(false, error, null);
}
