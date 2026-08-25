namespace MTGWantList.Models.Catalogue;

public class PrintingVariant
{
    // Internal database ID used by our application.
    public int Id { get; set; }

    // Language of this particular variant.
    //
    // Examples:
    // "English"
    // "Portuguese"
    // "Japanese"
    //
    // We are keeping this as a string for now.
    // We can normalise languages into their own table later if needed.
    public string Language { get; set; } = string.Empty;

    // Physical finish of this variant.
    //
    // Examples currently relevant to Magic include:
    // "nonfoil"
    // "foil"
    // "etched"
    //
    // We deliberately keep the MTGJSON value rather than reducing
    // everything to a simple true/false foil flag.
    public string Finish { get; set; } = string.Empty;

    // MTGJSON UUID associated with the language-specific version
    // when one is available.
    //
    // For English this may correspond to the main printing UUID.
    // Foreign-language entries can have their own UUID.
    public string? MtgJsonUuid { get; set; }

    // MTGJSON can provide an identifier for a specific finish version.
    //
    // This is separate from MtgJsonUuid because the language record
    // and the physical foil/nonfoil version are not necessarily
    // represented by the same identifier upstream.
    public string? MtgJsonVersionId { get; set; }

    // Scryfall identifier for this language-specific version
    // when MTGJSON provides one.
    public string? ScryfallId { get; set; }

    // Foreign key pointing to the parent printing.
    public int PrintingId { get; set; }

    // Navigation property for the related printing.
    public Printing Printing { get; set; } = null!;
}