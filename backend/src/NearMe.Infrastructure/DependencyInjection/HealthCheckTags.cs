namespace NearMe.Infrastructure.DependencyInjection;

public static class HealthCheckTags
{
    /// <summary>Checks that must pass before the API can serve traffic (e.g. database connectivity).</summary>
    public const string Ready = "ready";
}
