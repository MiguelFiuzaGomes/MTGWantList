using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonForeignData
{
    // MTGJSON UUID for this language-specific version.
    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = string.Empty;

    // Language name as provided by MTGJSON.
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    // Localised card name.
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // External identifiers belonging specifically
    // to this language variant.
    [JsonPropertyName("identifiers")]
    public MtgJsonIdentifiers Identifiers { get; set; } = new();
}