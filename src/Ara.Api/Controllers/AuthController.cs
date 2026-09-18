using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Ara.Application.Auth;

namespace Ara.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegistrationPendingResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return result.Succeeded
            ? Ok(result.Pending)
            : Conflict(new { message = result.Error });
    }

    [HttpPost("resend-verification")]
    public async Task<ActionResult<RegistrationPendingResponse>> ResendVerification(ResendVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ResendVerificationAsync(request, cancellationToken);
        return result.Succeeded
            ? Ok(result.Pending)
            : BadRequest(new { message = result.Error });
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<AuthResponse>> VerifyEmail(VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.VerifyEmailAsync(request, cancellationToken);
        return result.Succeeded
            ? Ok(result.Response)
            : BadRequest(new { message = result.Error });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return result.Succeeded
            ? Ok(result.Response)
            : Unauthorized(new { message = result.Error });
    }
}
