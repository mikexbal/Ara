using Microsoft.Extensions.DependencyInjection;
using Ara.Application.Auth;
using Ara.Application.Destinations;

namespace Ara.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDestinationService, DestinationService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
