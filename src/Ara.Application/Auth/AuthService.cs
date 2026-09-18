using Ara.Application.Common.Interfaces;
using Ara.Domain.Entities;

namespace Ara.Application.Auth;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IVerificationCodeStore verificationCodeStore,
    IEmailSender emailSender) : IAuthService
{
    // Intentionally identical for "no such user" and "wrong password" — a distinct message
    // for either case lets an attacker enumerate which emails have accounts.
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private static readonly TimeSpan VerificationCodeLifetime = TimeSpan.FromSeconds(20);

    public async Task<RegisterResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(request.Email);

        var existing = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            return RegisterResult.Failure("Email is already registered.");
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            EmailConfirmed = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await userRepository.AddAsync(user, cancellationToken);
        await SendVerificationCodeAsync(normalizedEmail, cancellationToken);

        return RegisterResult.Success(new RegistrationPendingResponse(normalizedEmail, (int)VerificationCodeLifetime.TotalSeconds));
    }

    public async Task<RegisterResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(request.Email);

        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            return RegisterResult.Failure("No pending registration found for this email.");
        }
        if (user.EmailConfirmed)
        {
            return RegisterResult.Failure("This email is already verified.");
        }

        await SendVerificationCodeAsync(normalizedEmail, cancellationToken);

        return RegisterResult.Success(new RegistrationPendingResponse(normalizedEmail, (int)VerificationCodeLifetime.TotalSeconds));
    }

    public async Task<AuthResult> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(request.Email);

        if (!verificationCodeStore.TryGet(normalizedEmail, out var expectedCode) || expectedCode is null)
        {
            return AuthResult.Failure("Email not verified — that code has expired. Request a new one.");
        }
        if (!string.Equals(expectedCode, request.Code, StringComparison.Ordinal))
        {
            return AuthResult.Failure("Incorrect verification code.");
        }

        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            return AuthResult.Failure("Account not found.");
        }

        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);

        verificationCodeStore.Remove(normalizedEmail);

        return AuthResult.Success(BuildResponse(user));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(request.Email);

        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return AuthResult.Failure(InvalidCredentialsMessage);
        }

        return AuthResult.Success(BuildResponse(user));
    }

    private async Task SendVerificationCodeAsync(string email, CancellationToken cancellationToken)
    {
        var code = Random.Shared.Next(100_000, 1_000_000).ToString();
        verificationCodeStore.Set(email, code, VerificationCodeLifetime);

        var html = $"""
            <p>Your Ara verification code is:</p>
            <h2 style="letter-spacing:4px">{code}</h2>
            <p>This code expires in {(int)VerificationCodeLifetime.TotalSeconds} seconds.</p>
            """;

        await emailSender.SendAsync(email, "Verify your email — Ara", html, cancellationToken);
    }

    private AuthResponse BuildResponse(User user)
    {
        var token = jwtTokenGenerator.GenerateToken(user);
        var userDto = new UserDto(user.Id, user.FirstName, user.LastName, user.Email, user.EmailConfirmed);
        return new AuthResponse(token, userDto);
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
