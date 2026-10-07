using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace NearMe.IntegrationTests.Fixtures;

/// <summary>Hosts the API in memory against the real SQL Server test database.</summary>
public sealed class SqlServerApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:NearMeDatabase"] = SqlServerTestDatabase.ConnectionString,
            });
        });
    }
}
