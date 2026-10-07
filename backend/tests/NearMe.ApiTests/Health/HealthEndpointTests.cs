using System.Net;
using System.Text.Json;
using NearMe.ApiTests.Fixtures;

namespace NearMe.ApiTests.Health;

public sealed class HealthEndpointTests(NearMeApiFactory factory) : IClassFixture<NearMeApiFactory>
{
    [Fact]
    public async Task Liveness_returns_200_with_healthy_status()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("healthy", json.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Liveness_does_not_depend_on_the_database()
    {
        // The factory points at an unreachable database; liveness must still report healthy,
        // otherwise a database outage would make the platform restart a healthy process.
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Readiness_returns_503_when_database_is_unreachable()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("unhealthy", json.RootElement.GetProperty("status").GetString());
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/health/ready")]
    public async Task Health_responses_expose_only_an_overall_status(string path)
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();

        using var json = JsonDocument.Parse(body);
        var propertyNames = json.RootElement.EnumerateObject().Select(property => property.Name).ToArray();
        Assert.Equal(new[] { "status" }, propertyNames);

        // No dependency names, exception text or connection details.
        Assert.DoesNotContain("127.0.0.1", body, StringComparison.Ordinal);
        Assert.DoesNotContain("database", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("exception", body, StringComparison.OrdinalIgnoreCase);
    }
}
