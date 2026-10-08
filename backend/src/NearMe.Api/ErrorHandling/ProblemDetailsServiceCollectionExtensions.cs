using System.Diagnostics;

namespace NearMe.Api.ErrorHandling;

public static class ProblemDetailsServiceCollectionExtensions
{
    /// <summary>
    /// Registers consistent Problem Details error responses (see API Design Specification §31).
    /// </summary>
    public static IServiceCollection AddNearMeProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions.TryAdd(
                    "traceId",
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
