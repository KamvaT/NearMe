using Microsoft.EntityFrameworkCore;
using NearMe.Infrastructure.Persistence;

namespace NearMe.IntegrationTests.Fixtures;

public static class SqlServerTestDatabase
{
    /// <summary>
    /// Connection string for a disposable test database. Never point this at a real or shared database:
    /// integration tests are allowed to create and modify its schema.
    /// </summary>
    public const string ConnectionStringVariable = "NEARME_TEST_SQL_CONNECTION";

    public static string? ConnectionString => Environment.GetEnvironmentVariable(ConnectionStringVariable);

    public static bool IsConfigured => !string.IsNullOrWhiteSpace(ConnectionString);

    public static NearMeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<NearMeDbContext>()
            .UseSqlServer(ConnectionString ?? throw new InvalidOperationException($"{ConnectionStringVariable} is not set."))
            .Options;

        return new NearMeDbContext(options);
    }
}
