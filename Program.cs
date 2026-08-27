using MTGWantList.Models.MtgJson;
using MTGWantList.Services.MtgJson;

using Microsoft.EntityFrameworkCore;
using MTGWantList.Data;

using MTGWantList.Models.Catalogue;

using MTGWantList.Models.Import;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET's OpenAPI support.
// This will later let us inspect and test our API endpoints.
builder.Services.AddOpenApi();

// Register MtgJsonClient with ASP.NET's dependency injection system.
//
// AddHttpClient automatically creates and manages the HttpClient
// that MtgJsonClient requires in its constructor.
builder.Services.AddHttpClient<MtgJsonClient>();

// Register the MTGJSON catalogue importer.
//
// Scoped means ASP.NET creates one importer for each HTTP request.
// This matches AppDbContext's lifetime, since the importer uses
// the database context internally.
builder.Services.AddScoped<MtgJsonCatalogueImporter>();

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



// Temporary endpoint used to verify that EF Core can connect
// successfully to our PostgreSQL database.
//
// Example:
// GET /api/database/test
app.MapGet("/api/database/test",
    async (AppDbContext dbContext) =>
    {
        // Ask EF Core whether it can open a connection
        // to the configured PostgreSQL database.
        bool canConnect = await dbContext.Database.CanConnectAsync();

        if (!canConnect)
        {
            return Results.Problem(
                "Could not connect to the PostgreSQL database.");
        }

        return Results.Ok("Database connection successful.");
    });

// Temporary endpoint used to import one MTGJSON set
// into our PostgreSQL catalogue.
//
// The importer now returns a summary describing what was
// processed and what new records were actually created.
//
// Example:
// POST /api/mtgjson/import/FDN
app.MapPost("/api/mtgjson/import/{setCode}",
    async (
        string setCode,
        MtgJsonCatalogueImporter importer) =>
    {
        // Run the import and collect its statistics.
        CatalogueImportResult result =
            await importer.ImportSetAsync(setCode);

        // Return the result directly as JSON.
        return Results.Ok(result);
    });

// Temporary catalogue statistics endpoint.
//
// This lets us verify that an import created the expected
// records in each catalogue table.
//
// Example:
// GET /api/catalogue/stats
app.MapGet("/api/catalogue/stats",
    async (AppDbContext dbContext) =>
    {
        // Count the records currently stored in each
        // of our catalogue tables.
        int cardSetCount =
            await dbContext.CardSets.CountAsync();

        int cardCount =
            await dbContext.Cards.CountAsync();

        int printingCount =
            await dbContext.Printings.CountAsync();

        int variantCount =
            await dbContext.PrintingVariants.CountAsync();

        return Results.Ok(new
        {
            CardSets = cardSetCount,
            Cards = cardCount,
            Printings = printingCount,
            PrintingVariants = variantCount
        });
    });

// Temporary endpoint used to inspect catalogue data that has
// actually been imported into PostgreSQL.
//
// Example:
// GET /api/catalogue/card/Llanowar%20Elves
app.MapGet("/api/catalogue/card/{cardName}",
    async (string cardName, AppDbContext dbContext) =>
    {
        // Find the requested card and load its related printings,
        // sets and physical language/finish variants.
        Card? card = await dbContext.Cards
            .AsNoTracking()
            .Where(card =>
                card.Name == cardName)
            .Select(card => new Card
            {
                Id = card.Id,
                Name = card.Name,
                ScryfallOracleId = card.ScryfallOracleId
            })
            .FirstOrDefaultAsync();

        if (card is null)
        {
            return Results.NotFound();
        }

        // Retrieve all printings belonging to this card.
        var printings = await dbContext.Printings
            .AsNoTracking()
            .Where(printing =>
                printing.CardId == card.Id)
            .Select(printing => new
            {
                printing.Id,
                printing.CollectorNumber,
                printing.Rarity,

                Set = printing.CardSet.Name,
                SetCode = printing.CardSet.MtgJsonCode,

                Variants = dbContext.PrintingVariants
                    .Where(variant =>
                        variant.PrintingId == printing.Id)
                    .Select(variant => new
                    {
                        variant.Language,
                        variant.Finish,
                        variant.MtgJsonVersionId
                    })
                    .ToList()
            })
            .ToListAsync();

        return Results.Ok(new
        {
            card.Id,
            card.Name,
            card.ScryfallOracleId,
            Printings = printings
        });
    });

// Temporary endpoint used to inspect MTGJSON's set list.
//
// This lets us verify the SetList model and see which
// set types / online-only flags MTGJSON actually provides.
//
// Example:
// GET /api/mtgjson/sets
app.MapGet("/api/mtgjson/sets",
    async (MtgJsonClient mtgJsonClient) =>
    {
        MtgJsonSetListResponse? response =
            await mtgJsonClient.GetSetListAsync();

        if (response is null)
        {
            return Results.NotFound(
                "MTGJSON returned no set list.");
        }

        // Return a reduced view rather than dumping every
        // property from every set.
        var sets = response.Data
            .Select(set => new
            {
                set.Code,
                set.Name,
                set.Type,
                set.ReleaseDate,
                set.IsOnlineOnly,
                set.IsPaperOnly
            })
            .ToList();

        return Results.Ok(new
        {
            Count = sets.Count,
            Sets = sets
        });
    });

// Start the ASP.NET application.
app.Run();