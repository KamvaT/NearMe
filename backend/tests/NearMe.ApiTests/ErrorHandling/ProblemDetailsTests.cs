using System.Net;
using System.Text.Json;
using NearMe.ApiTests.Fixtures;

namespace NearMe.ApiTests.ErrorHandling;

public sealed class ProblemDetailsTests(NearMeApiFactory factory) : IClassFixture<NearMeApiFactory>
{
    [Fact]
    public async Task Unknown_route_returns_404_problem_details()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/v1/route-that-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(404, json.RootElement.GetProperty("status").GetInt32());
        Assert.True(json.RootElement.TryGetProperty("traceId", out _), "Problem Details must carry a traceId.");
    }

    [Fact]
    public async Task OpenApi_document_is_not_published_outside_development()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
