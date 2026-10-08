using System.Net;
using System.Text.Json;
using NearMe.IntegrationTests.Fixtures;

namespace NearMe.IntegrationTests.Persistence;

/// <remarks>
/// Phase 1 verifies the EF Core → SQL Server path end to end. EnsureCreated is used only because
/// no migrations exist yet; once the first migration is added (docs/adr/0004) these tests switch to
/// Database.MigrateAsync() and gain a "no pending model changes" check.
/// </remarks>
public sealed class DatabaseConnectivityTests
{
    [RequiresSqlServerFact]
    public async Task NearMeDbContext_can_connect_to_sql_server()
    {
        await using var dbContext = SqlServerTestDatabase.CreateDbContext();

        await dbContext.Database.EnsureCreatedAsync();

        Assert.True(await dbContext.Database.CanConnectAsync());
    }

    [RequiresSqlServerFact]
    public async Task Readiness_probe_reports_healthy_when_database_is_reachable()
    {
        await using (var dbContext = SqlServerTestDatabase.CreateDbContext())
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        await using var factory = new SqlServerApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("healthy", json.RootElement.GetProperty("status").GetString());
    }
}
