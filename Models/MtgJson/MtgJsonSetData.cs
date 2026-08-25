using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonSetData
{
    // MTGJSON set code, for example "FDN".
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    // Human-readable set name.
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    // Release date as provided by MTGJSON.
    // Keeping this as a string initially avoids unnecessary parsing decisions.
    [JsonPropertyName("releaseDate")]
    public string? ReleaseDate { get; set; }

    // Cards contained in this set.
    [JsonPropertyName("cards")]
    public List<MtgJsonCard> Cards { get; set; } = [];
}