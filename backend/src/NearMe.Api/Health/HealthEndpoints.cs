using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NearMe.Infrastructure.DependencyInjection;

namespace NearMe.Api.Health;

public static class HealthEndpoints
{
    /// <summary>
    /// Maps the unversioned infrastructure probes (see docs/adr/0003):
    /// <list type="bullet">
    ///   <item><c>/health</c> — liveness: the API process is running. No dependency checks.</item>
    ///   <item><c>/health/ready</c> — readiness: required dependencies (database) are reachable.</item>
    /// </list>
    /// Responses contain only an overall status. Check names, exception messages and
    /// connection details are never written to the response.
    /// </summary>
    public static IEndpointRouteBuilder MapNearMeHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = WriteStatusOnlyAsync,
        }).AllowAnonymous();

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains(HealthCheckTags.Ready),
            ResponseWriter = WriteStatusOnlyAsync,
        }).AllowAnonymous();

        return endpoints;
    }

    private static Task WriteStatusOnlyAsync(HttpContext context, HealthReport report)
    {
        var status = report.Status switch
        {
            HealthStatus.Healthy => "healthy",
            HealthStatus.Degraded => "degraded",
            _ => "unhealthy",
        };

        return context.Response.WriteAsJsonAsync(new HealthResponse(status));
    }

    private sealed record HealthResponse(string Status);
}
