using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using Ara.Application.Common.Interfaces;
using Ara.Infrastructure.Email;
using Ara.Infrastructure.Persistence;
using Ara.Infrastructure.Security;
using Ara.Infrastructure.Storage;

namespace Ara.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AraDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AraDb")));
        services.AddScoped<IDestinationRepository, PostgresDestinationRepository>();
        services.AddScoped<IUserRepository, PostgresUserRepository>();

        services.Configure<R2Options>(configuration.GetSection(R2Options.SectionName));
        services.AddHttpClient("CloudflareR2");
        services.AddMemoryCache();
        services.AddSingleton<IImageStorageService, CloudflareR2ImageStorageService>();
        services.AddHostedService<ImageCacheWarmupService>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IVerificationCodeStore, MemoryVerificationCodeStore>();

        services.Configure<ResendOptions>(configuration.GetSection(ResendOptions.SectionName));
        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = configuration.GetSection(ResendOptions.SectionName)["ApiKey"] ?? string.Empty;
        });
        services.AddTransient<IResend, ResendClient>();
        services.AddScoped<IEmailSender, ResendEmailSender>();

        return services;
    }
}
