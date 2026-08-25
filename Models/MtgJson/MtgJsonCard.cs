using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonCard
{
    // MTGJSON's unique identifier for this printing.
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    // Card name.
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Collector number within the set.
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    // Card rarity, for example common, uncommon, rare or mythic.
    [JsonPropertyName("rarity")]
    public string? Rarity { get; set; }

    // Language of this printing.
    [JsonPropertyName("language")]
    public string? Language { get; set; }
}