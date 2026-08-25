namespace MTGWantList.Models.Catalogue;

public class Printing
{
    // Internal database ID used by our application.
    public int Id { get; set; }

    // MTGJSON UUID for this specific printing.
    //
    // This is useful when updating the catalogue because it gives us
    // a stable way to identify the same MTGJSON printing again.
    public string MtgJsonUuid { get; set; } = string.Empty;

    // Collector number within the set.
    //
    // This is a string rather than an integer because Magic collector
    // numbers are not guaranteed to contain only numbers.
    public string CollectorNumber { get; set; } = string.Empty;

    // Rarity of this particular printing.
    //
    // Rarity belongs here rather than on Card because the same card
    // can have different rarities in different sets.
    public string? Rarity { get; set; }

    // Scryfall identifier for this particular printing.
    public string? ScryfallId { get; set; }

    // Cardmarket product identifier when MTGJSON provides one.
    public string? CardmarketId { get; set; }

    // Foreign key pointing to the underlying card.
    public int CardId { get; set; }

    // Navigation property allowing EF Core to load the related Card.
    public Card Card { get; set; } = null!;

    // Foreign key pointing to the set containing this printing.
    public int CardSetId { get; set; }

    // Navigation property allowing EF Core to load the related set.
    public CardSet CardSet { get; set; } = null!;
}