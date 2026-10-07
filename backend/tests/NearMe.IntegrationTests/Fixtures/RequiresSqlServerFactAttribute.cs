namespace NearMe.IntegrationTests.Fixtures;

/// <summary>
/// A [Fact] that runs only when a SQL Server test database is configured. Without it the test
/// is reported as Skipped (never silently passed), so a missing database is always visible.
/// </summary>
public sealed class RequiresSqlServerFactAttribute : FactAttribute
{
    public RequiresSqlServerFactAttribute()
    {
        if (!SqlServerTestDatabase.IsConfigured)
        {
            Skip = $"Set {SqlServerTestDatabase.ConnectionStringVariable} to a dedicated, disposable SQL Server test database to run this test.";
        }
    }
}
