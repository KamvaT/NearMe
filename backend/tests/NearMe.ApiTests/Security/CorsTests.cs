using NearMe.ApiTests.Fixtures;

namespace NearMe.ApiTests.Security;

/// <summary>
/// CORS must allow only configured frontend origins (API Design Specification §37).
/// Note: CORS is a browser-enforced control, not an authorization boundary; these tests
/// verify configuration, while authorization is enforced server-side per request.
/// </summary>
public sealed class CorsTests(NearMeApiFactory factory) : IClassFixture<NearMeApiFactory>
{
    private const string AllowOriginHeader = "Access-Control-Allow-Origin";

    [Fact]
    public async Task Configured_frontend_origin_is_allowed()
    {
        using var response = await GetHealthFromOriginAsync(NearMeApiFactory.AllowedOrigin);

        Assert.True(response.Headers.TryGetValues(AllowOriginHeader, out var values));
        Assert.Equal(NearMeApiFactory.AllowedOrigin, Assert.Single(values));
    }

    [Theory]
    [InlineData("https://evil.example")]
    [InlineData("http://localhost:5174")]
    [InlineData("null")]
    public async Task Unconfigured_origin_is_not_allowed(string origin)
    {
        using var response = await GetHealthFromOriginAsync(origin);

        Assert.False(response.Headers.Contains(AllowOriginHeader));
    }

    private async Task<HttpResponseMessage> GetHealthFromOriginAsync(string origin)
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("Origin", origin);

        return await client.SendAsync(request);
    }
}
