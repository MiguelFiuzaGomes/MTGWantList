using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonSetListEntry
{
    // MTGJSON set code.
    // This also corresponds to the individual set filename.
    //
    // Example:
    // FDN -> FDN.json
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    // Human-readable set name.
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // MTGJSON's classification for this set.
    //
    // Examples include expansion, core, commander,
    // promo and other product types.
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    // Release date supplied by MTGJSON.
    [JsonPropertyName("releaseDate")]
    public string ReleaseDate { get; set; } = string.Empty;

    // True when the set only exists digitally.
    //
    // This will become useful shortly because we may decide
    // not to import MTGO/Arena-only products into a physical
    // shop catalogue.
    [JsonPropertyName("isOnlineOnly")]
    public bool IsOnlineOnly { get; set; }

    // MTGJSON provides this for sets known to be paper-only.
    // It is nullable because the property itself is optional.
    [JsonPropertyName("isPaperOnly")]
    public bool? IsPaperOnly { get; set; }
}