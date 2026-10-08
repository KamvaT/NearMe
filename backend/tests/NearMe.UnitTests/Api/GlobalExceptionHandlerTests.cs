using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NearMe.Api.ErrorHandling;

namespace NearMe.UnitTests.Api;

public sealed class GlobalExceptionHandlerTests
{
    private const string SensitiveDetail = "Server=prod-sql.internal;Password=hunter2";

    [Fact]
    public async Task Unhandled_exception_returns_generic_500_problem_details()
    {
        var (handled, httpContext, body) = await HandleAsync(new InvalidOperationException(SensitiveDetail));

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.StartsWith("application/problem+json", httpContext.Response.ContentType);

        using var json = JsonDocument.Parse(body);
        Assert.Equal(500, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("An unexpected error occurred.", json.RootElement.GetProperty("title").GetString());
        Assert.True(json.RootElement.TryGetProperty("traceId", out _), "Problem Details must carry a traceId for support correlation.");
    }

    [Fact]
    public async Task Exception_message_and_stack_trace_are_not_returned_to_the_client()
    {
        var (_, _, body) = await HandleAsync(new InvalidOperationException(SensitiveDetail));

        Assert.DoesNotContain("hunter2", body, StringComparison.Ordinal);
        Assert.DoesNotContain("prod-sql", body, StringComparison.Ordinal);
        Assert.DoesNotContain(nameof(InvalidOperationException), body, StringComparison.Ordinal);
        Assert.DoesNotContain("   at ", body, StringComparison.Ordinal);
    }

    private static async Task<(bool Handled, HttpContext Context, string Body)> HandleAsync(Exception exception)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddNearMeProblemDetails()
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = services };
        httpContext.Request.Method = HttpMethods.Get;
        httpContext.Request.Path = "/api/v1/example";
        using var responseBody = new MemoryStream();
        httpContext.Response.Body = responseBody;

        var handler = new GlobalExceptionHandler(
            services.GetRequiredService<IProblemDetailsService>(),
            NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        return (handled, httpContext, System.Text.Encoding.UTF8.GetString(responseBody.ToArray()));
    }
}
