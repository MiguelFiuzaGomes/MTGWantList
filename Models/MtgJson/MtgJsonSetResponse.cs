using System.Text.Json.Serialization;

namespace MTGWantList.Models.MtgJson;

public class MtgJsonSetResponse
{
    //NTGJSON wraps the actual set contents inside the "data" property
    [JsonPropertyName("data")] public MtgJsonSetData Data { get; set; } = new();

}