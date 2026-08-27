namespace MTGWantList.Models.Import;

public class CatalogueImportResult
{
    // MTGJSON set code that was imported.
    // Example: "FDN".
    public string SetCode { get; set; } = string.Empty;

    // Number of MTGJSON card/printing records processed
    // while importing this set.
    public int CardsProcessed { get; set; }

    // Number of new Card records created in our catalogue.
    public int CardsCreated { get; set; }

    // Number of new Printing records created.
    public int PrintingsCreated { get; set; }

    // Number of new language + finish variants created.
    public int VariantsCreated { get; set; }
}