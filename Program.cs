using MTGWantList.Services.MtgJson;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET's OpenAPI support.
// This will later let us inspect and test our API endpoints.
builder.Services.AddOpenApi();

// Register MtgJsonClient with ASP.NET's dependency injection system.
//
// AddHttpClient automatically creates and manages the HttpClient
// that MtgJsonClient requires in its constructor.
builder.Services.AddHttpClient<MtgJsonClient>();

var app = builder.Build();

// Only expose the generated OpenAPI document while developing.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Temporary test endpoint for the MTGJSON client.
//
// The {setCode} part of the route is supplied by the user.
// ASP.NET also provides our registered MtgJsonClient automatically
// through dependency injection.
//
// Example request:
// GET /api/mtgjson/set/FDN
app.MapGet("/api/mtgjson/set/{setCode}",
    async (string setCode, MtgJsonClient mtgJsonClient) =>
    {
        // Ask the MTGJSON client to download the requested set.
        string json = await mtgJsonClient.GetSetAsync(setCode);

        // Return the MTGJSON response as JSON rather than plain text.
        return Results.Content(json, "application/json");
    });

// Start the ASP.NET application.
app.Run();