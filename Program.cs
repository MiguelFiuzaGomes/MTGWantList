using MTGWantList.Models.MtgJson;
using MTGWantList.Services.MtgJson;

using Microsoft.EntityFrameworkCore;
using MTGWantList.Data;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET's OpenAPI support.
// This will later let us inspect and test our API endpoints.
builder.Services.AddOpenApi();

// Register MtgJsonClient with ASP.NET's dependency injection system.
//
// AddHttpClient automatically creates and manages the HttpClient
// that MtgJsonClient requires in its constructor.
builder.Services.AddHttpClient<MtgJsonClient>();

// Register the application's EF Core database context.
//
// UseNpgsql tells Entity Framework Core that PostgreSQL
// is the database provider we want to use.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.MapGet("/api/mtgjson/set/{setCode}/card/{cardName}",
    async (string setCode, string cardName, MtgJsonClient mtgJsonClient) =>
    {
        MtgJsonSetResponse? set =
            await mtgJsonClient.GetSetAsync(setCode);

        if (set is null)
        {
            return Results.NotFound();
        }

        // Find the first card whose full name matches the supplied value,
        // or whose combined name begins with the supplied front-face name.
        //
        // This is useful for double-faced cards such as:
        // "Delver of Secrets // Insectile Aberration"
        MtgJsonCard? card = set.Data.Cards.FirstOrDefault(card =>
        string.Equals(
            card.Name,
            cardName,
            StringComparison.OrdinalIgnoreCase)
        ||
        card.Name.StartsWith(
            $"{cardName} //",
            StringComparison.OrdinalIgnoreCase));

        if (card is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(card);
    });


// Start the ASP.NET application.
app.Run();