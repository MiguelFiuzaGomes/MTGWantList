using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonIdentifiers
{
    // Scryfall identifier for this printing.
    [JsonPropertyName("scryfallId")]
    public string? ScryfallId { get; set; }

    // Scryfall Oracle ID identifies the underlying card
    // independently of a particular printing.
    [JsonPropertyName("scryfallOracleId")]
    public string? ScryfallOracleId { get; set; }

    // Cardmarket / MagicCardMarket product identifier.
    [JsonPropertyName("mcmId")]
    public string? CardmarketId { get; set; }

    // MTGJSON maintains separate identifiers for foil
    // and non-foil versions where applicable.
    [JsonPropertyName("mtgjsonFoilVersionId")]
    public string? MtgJsonFoilVersionId { get; set; }

    [JsonPropertyName("mtgjsonNonFoilVersionId")]
    public string? MtgJsonNonFoilVersionId { get; set; }
}