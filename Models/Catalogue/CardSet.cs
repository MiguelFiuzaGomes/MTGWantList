namespace MTGWantList.Models.Catalogue;

public class CardSet
{
    // Internal database ID.
    // This belongs to our application and is independent of MTGJSON.
    public int Id { get; set; }

    // MTGJSON's set code.
    // Examples: "FDN", "ISD", "ZNR".
    public string MtgJsonCode { get; set; } = string.Empty;

    // Human-readable set name.
    // Example: "Magic: The Gathering Foundations".
    public string Name { get; set; } = string.Empty;

    // Official release date of the set.
    // Nullable because some unusual or upcoming sets may not
    // provide a usable release date.
    public DateOnly? ReleaseDate { get; set; }
}