using Microsoft.EntityFrameworkCore;
using MTGWantList.Data;
using MTGWantList.Models.Catalogue;
using MTGWantList.Models.MtgJson;
using MTGWantList.Models.Import;

namespace MTGWantList.Services.MtgJson;

public class MtgJsonCatalogueImporter
{
    private readonly MtgJsonClient _mtgJsonClient;
    private readonly AppDbContext _dbContext;

    // ASP.NET will provide both the MTGJSON client and our database
    // context through dependency injection.
    public MtgJsonCatalogueImporter(
        MtgJsonClient mtgJsonClient,
        AppDbContext dbContext)
    {
        _mtgJsonClient = mtgJsonClient;
        _dbContext = dbContext;
    }

    // Imports one complete Magic set from MTGJSON into our catalogue.
    //
    // For our first version this is intentionally simple:
    // existing printings are skipped rather than updated.
    // We can add proper catalogue synchronisation later.
    public async Task<CatalogueImportResult> ImportSetAsync(string setCode)    {
        // Download and deserialize the set from MTGJSON.
        MtgJsonSetResponse? response =
            await _mtgJsonClient.GetSetAsync(setCode);

        if (response is null)
        {
            throw new InvalidOperationException(
                $"MTGJSON returned no data for set '{setCode}'.");
        }

        MtgJsonSetData sourceSet = response.Data;
        
        // Tracks what happened during this specific import.
        // This will later be useful when importing many sets in sequence.
        CatalogueImportResult result = new()
        {
            SetCode = sourceSet.Code
        };


        // ------------------------------------------------------------
        // CardSet
        // ------------------------------------------------------------

        // Convert MTGJSON's release-date string into our DateOnly value.
        DateOnly? releaseDate = null;

        if (DateOnly.TryParse(
                sourceSet.ReleaseDate,
                out DateOnly parsedReleaseDate))
        {
            releaseDate = parsedReleaseDate;
        }

        // Check whether this set already exists in our catalogue.
        CardSet? cardSet = await _dbContext.CardSets
            .SingleOrDefaultAsync(
                set => set.MtgJsonCode == sourceSet.Code);

        if (cardSet is null)
        {
            // First time we have seen this set.
            cardSet = new CardSet
            {
                MtgJsonCode = sourceSet.Code,
                Name = sourceSet.Name,
                ReleaseDate = releaseDate
            };

            _dbContext.CardSets.Add(cardSet);
        }
        else
        {
            // Keep basic set information current if the importer
            // is run again later.
            cardSet.Name = sourceSet.Name;
            cardSet.ReleaseDate = releaseDate;
        }


        // ------------------------------------------------------------
        // Cards and Printings
        // ------------------------------------------------------------

        foreach (MtgJsonCard sourceCard in sourceSet.Cards)
        {
            // Every MTGJSON card record represents a printing we attempted
            // to process during this import.
            result.CardsProcessed++;
            
            string? oracleId =
                sourceCard.Identifiers.ScryfallOracleId;

            Card? card;

            if (!string.IsNullOrWhiteSpace(oracleId))
            {
                // First check entities that EF Core is already tracking locally.
                //
                // This is important because another printing in the same import
                // may already have created this Card, even though SaveChanges()
                // has not yet written it to PostgreSQL.
                card = _dbContext.Cards.Local
                    .FirstOrDefault(existingCard =>
                        existingCard.ScryfallOracleId == oracleId);

                // If EF is not already tracking the card, check PostgreSQL.
                card ??= await _dbContext.Cards
                    .SingleOrDefaultAsync(existingCard =>
                        existingCard.ScryfallOracleId == oracleId);
            }
            else
            {
                // Some unusual MTGJSON records may not provide an Oracle ID.
                //
                // Check locally first for the same reason as above.
                card = _dbContext.Cards.Local
                    .FirstOrDefault(existingCard =>
                        existingCard.ScryfallOracleId == null &&
                        existingCard.Name == sourceCard.Name);

                // If it is not currently tracked, check the database.
                card ??= await _dbContext.Cards
                    .FirstOrDefaultAsync(existingCard =>
                        existingCard.ScryfallOracleId == null &&
                        existingCard.Name == sourceCard.Name);
            }

            if (card is null)
            {
                // This is genuinely a card we have not encountered before.
                card = new Card
                {
                    Name = sourceCard.Name,
                    ScryfallOracleId = oracleId
                };

                _dbContext.Cards.Add(card);
                
                // This printing did not previously exist in our catalogue.
                result.CardsCreated++;
                
            }
            else
            {
                // Keep the canonical name current when re-importing catalogue data.
                card.Name = sourceCard.Name;
            }
            
            // ------------------------------------------------------------
            // Printing
            // ------------------------------------------------------------

            // MTGJSON's UUID identifies this exact printing.
            //
            // Check both PostgreSQL and EF Core's local change tracker so
            // the same printing cannot accidentally be queued twice during
            // one import operation.
            bool printingAlreadyExists =
                _dbContext.Printings.Local.Any(
                    printing =>
                        printing.MtgJsonUuid == sourceCard.Uuid)
                ||
                await _dbContext.Printings.AnyAsync(
                    printing =>
                        printing.MtgJsonUuid == sourceCard.Uuid);

            if (printingAlreadyExists)
            {
                continue;
            }

            // Create the set-specific printing.
            //
            // Example:
            // Card: Llanowar Elves
            // Set: Foundations
            // Collector number: 227
            Printing printing = new()
            {
                MtgJsonUuid = sourceCard.Uuid,
                CollectorNumber = sourceCard.Number,
                Rarity = sourceCard.Rarity,
                ScryfallId = sourceCard.Identifiers.ScryfallId,
                CardmarketId = sourceCard.Identifiers.CardmarketId,

                // EF Core will use these navigation properties to populate
                // CardId and CardSetId automatically.
                Card = card,
                CardSet = cardSet
            };

            _dbContext.Printings.Add(printing);
            result.PrintingsCreated++;


            // ------------------------------------------------------------
            // Main-language variants
            // ------------------------------------------------------------

            // Create only the physical finish variants that MTGJSON
            // explicitly says exist for the main language.
            result.VariantsCreated += AddVariants(
                printing,
                sourceCard.Language,
                sourceCard.Uuid,
                sourceCard.Identifiers.ScryfallId,
                sourceCard.SkuIds);


            // ------------------------------------------------------------
            // Foreign-language variants
            // ------------------------------------------------------------

            foreach (MtgJsonForeignData foreignCard in sourceCard.ForeignData)
            {
                // Each translated card can have its own UUID, Scryfall ID
                // and available finish SKUs.
                result.VariantsCreated += AddVariants(
                    printing,
                    foreignCard.Language,
                    foreignCard.Uuid,
                    foreignCard.Identifiers.ScryfallId,
                    foreignCard.SkuIds);
            }
            
        }


        // Save everything for this set in one database operation.
        await _dbContext.SaveChangesAsync();

        // The import for this set is complete.
        //
        // EF Core normally keeps every loaded/created entity in its change tracker.
        // When we later import hundreds of sets in sequence, allowing those tracked
        // entities to accumulate would waste a significant amount of memory.
        //
        // Clearing the tracker here is safe because all changes have already
        // been saved to PostgreSQL.
        _dbContext.ChangeTracker.Clear();

        // Return the summary of what happened during this import.
        return result;
    }


    // Creates the concrete finish variants that MTGJSON says
    // actually exist for one language.
    private int AddVariants(
        Printing printing,
        string language,
        string mtgJsonUuid,
        string? scryfallId,
        MtgJsonSkuIds skuIds)
    {
        int variantsCreated = 0;

        // Count each finish only when MTGJSON confirms
        // that a real SKU exists for it.
        variantsCreated += AddVariant(printing, language, "nonfoil",
            mtgJsonUuid, scryfallId, skuIds.Nonfoil);

        variantsCreated += AddVariant(printing, language, "foil",
            mtgJsonUuid, scryfallId, skuIds.Foil);

        variantsCreated += AddVariant(printing, language, "etched",
            mtgJsonUuid, scryfallId, skuIds.Etched);

        return variantsCreated;
    }


    // Adds one PrintingVariant only when MTGJSON supplies
    // an actual SKU ID for that language + finish combination.
    private int AddVariant(
        Printing printing,
        string language,
        string finish,
        string mtgJsonUuid,
        string? scryfallId,
        string? versionId)
    {
        // No SKU means this language + finish combination
        // is not confirmed to exist.
        if (string.IsNullOrWhiteSpace(versionId))
        {
            return 0;
        }

        PrintingVariant variant = new()
        {
            Printing = printing,
            Language = language,
            Finish = finish,
            MtgJsonUuid = mtgJsonUuid,
            MtgJsonVersionId = versionId,
            ScryfallId = scryfallId
        };

        _dbContext.PrintingVariants.Add(variant);

        // One real variant was queued for insertion.
        return 1;
    }
}