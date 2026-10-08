using Microsoft.AspNetCore.Cors.Infrastructure;

namespace NearMe.Api.Configuration;

public static class CorsConfiguration
{
    public const string FrontendPolicy = "Frontend";

    /// <summary>Configuration key holding the trusted frontend origins (array of absolute origins).</summary>
    public const string AllowedOriginsKey = "Cors:AllowedOrigins";

    /// <summary>
    /// Allows only explicitly configured frontend origins. There is deliberately no
    /// AllowAnyOrigin fallback: if no origins are configured, cross-origin requests are refused.
    /// </summary>
    public static IServiceCollection AddNearMeCors(this IServiceCollection services)
    {
        services.AddCors();

        services.AddOptions<CorsOptions>()
            .Configure<IConfiguration>((options, configuration) =>
            {
                var allowedOrigins = configuration.GetSection(AllowedOriginsKey).Get<string[]>() ?? [];

                options.AddPolicy(FrontendPolicy, policy => policy
                    .WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                    .WithHeaders("Content-Type", "Authorization"));
            });

        return services;
    }
}
