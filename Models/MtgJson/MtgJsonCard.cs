using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonCard
{
    // MTGJSON's unique identifier for this particular printing.
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    // Oracle/card name used by the English printing.
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Collector number inside the set.
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    // Card rarity, for example common, uncommon, rare or mythic.
    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    // Available physical finishes for this printing.
    // MTGJSON can list multiple finishes for the same card.
    [JsonPropertyName("finishes")]
    public List<string> Finishes { get; set; } = [];

    // External identifiers such as Scryfall and Cardmarket IDs.
    [JsonPropertyName("identifiers")]
    public MtgJsonIdentifiers Identifiers { get; set; } = new();

    // Alternate-language versions of this printing.
    [JsonPropertyName("foreignData")]
    public List<MtgJsonForeignData> ForeignData { get; set; } = [];
    
    // Language of the main card record.
    // For most normal set files this will be "English".
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    // Contains the IDs of the physical finish variants
    // that actually exist for this language.
    [JsonPropertyName("skuIds")]
    public MtgJsonSkuIds SkuIds { get; set; } = new();
}