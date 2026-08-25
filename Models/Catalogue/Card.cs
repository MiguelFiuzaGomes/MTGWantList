namespace MTGWantList.Models.Catalogue;

public class Card
{
    // Internal database ID used by our application.
    public int Id { get; set; }

    // Canonical card name.
    //
    // For double-faced cards this can be the combined name, for example:
    // "Delver of Secrets // Insectile Aberration".
    public string Name { get; set; } = string.Empty;

    // Scryfall's Oracle ID identifies the underlying card
    // independently of a particular printing.
    //
    // This is useful when importing multiple printings of the same card
    // because they should all resolve back to one Card record.
    //
    // Nullable because we should not make our own database depend on
    // every external record always having a Scryfall identifier.
    public string? ScryfallOracleId { get; set; }
}