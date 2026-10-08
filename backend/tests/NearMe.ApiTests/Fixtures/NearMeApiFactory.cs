using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace NearMe.ApiTests.Fixtures;

/// <summary>
/// Hosts the real API pipeline in memory. Uses a non-Development environment so production-like
/// behaviour (no OpenAPI document, generic errors) is what gets tested, and supplies all
/// configuration explicitly so tests never depend on a developer's local settings or secrets.
/// </summary>
public sealed class NearMeApiFactory : WebApplicationFactory<Program>
{
    public const string AllowedOrigin = "http://localhost:5173";

    // Deliberately unreachable: port 1 on loopback refuses connections immediately.
    private const string UnreachableDatabase =
        "Server=127.0.0.1,1;Database=NearMe_ApiTests;User Id=nearme_test;Password=not-a-real-secret;Connect Timeout=2;Encrypt=False";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:NearMeDatabase"] = UnreachableDatabase,
                ["Cors:AllowedOrigins:0"] = AllowedOrigin,
            });
        });
    }
}
