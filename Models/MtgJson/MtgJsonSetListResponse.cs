using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonSetListResponse
{
    // MTGJSON wraps the list of available sets
    // inside the top-level "data" property.
    [JsonPropertyName("data")]
    public List<MtgJsonSetListEntry> Data { get; set; } = [];
}