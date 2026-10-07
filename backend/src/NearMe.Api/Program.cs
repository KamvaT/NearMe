using NearMe.Api.Configuration;
using NearMe.Api.ErrorHandling;
using NearMe.Api.Health;
using NearMe.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ---- Services ------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddNearMeProblemDetails();
builder.Services.AddNearMeCors();
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure();

var app = builder.Build();

// ---- HTTP pipeline (order matters) --------------------------------------------
app.UseExceptionHandler();   // Unhandled exceptions -> generic 500 Problem Details.
app.UseStatusCodePages();    // Empty 4xx/5xx responses (e.g. unknown route) -> Problem Details.

if (app.Environment.IsDevelopment())
{
    // The OpenAPI document is only published in Development (/openapi/v1.json).
    app.MapOpenApi();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(CorsConfiguration.FrontendPolicy);

app.MapNearMeHealthChecks();
app.MapControllers();

app.Run();

/// <summary>Exposed so the test projects can host the API with WebApplicationFactory.</summary>
public partial class Program
{
}
