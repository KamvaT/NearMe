using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NearMe.Infrastructure.Persistence;

namespace NearMe.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    /// <summary>Name of the connection string in configuration (ConnectionStrings:NearMeDatabase).</summary>
    public const string ConnectionStringName = "NearMeDatabase";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // The connection string is resolved when the DbContext is first created rather than at
        // startup, so configuration supplied by user-secrets, environment variables or test hosts
        // is always honoured. A missing value fails loudly instead of silently using a default.
        services.AddDbContext<NearMeDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured. " +
                    "Set it with user-secrets locally or an environment variable in deployed environments.");
            }

            options.UseSqlServer(connectionString);
        });

        services.AddHealthChecks()
            .AddDbContextCheck<NearMeDbContext>(name: "database", tags: [HealthCheckTags.Ready]);

        return services;
    }
}
