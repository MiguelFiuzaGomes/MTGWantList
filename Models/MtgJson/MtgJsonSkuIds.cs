using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonSkuIds
{
    // Unique MTGJSON ID for the etched version,
    // when this language was actually printed in etched foil.
    [JsonPropertyName("etched")]
    public string? Etched { get; set; }

    // Unique MTGJSON ID for the foil version.
    [JsonPropertyName("foil")]
    public string? Foil { get; set; }

    // Unique MTGJSON ID for the non-foil version.
    [JsonPropertyName("nonfoil")]
    public string? Nonfoil { get; set; }

    // Used by MTGJSON for finishes that do not fit
    // one of the standard finish categories.
    [JsonPropertyName("other")]
    public string? Other { get; set; }

    // Unique ID for signed versions, when applicable.
    [JsonPropertyName("signed")]
    public string? Signed { get; set; }
}