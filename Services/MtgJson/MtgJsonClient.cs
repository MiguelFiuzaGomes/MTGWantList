using System.Net.Http.Json;
using MTGWantList.Models.MtgJson;

namespace MTGWantList.Services.MtgJson;

public class MtgJsonClient
{
    private readonly HttpClient _httpClient;

    public MtgJsonClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Downloads and deserializes a specific MTGJSON set.
    public async Task<MtgJsonSetResponse?> GetSetAsync(string setCode)
    {
        string url =
            $"https://mtgjson.com/api/v5/{setCode.ToUpperInvariant()}.json";

        // GetFromJsonAsync downloads the response and uses System.Text.Json
        // to convert it directly into our C# model.
        return await _httpClient.GetFromJsonAsync<MtgJsonSetResponse>(url);
    }
    
    // Downloads and deserializes MTGJSON's list of available sets.
//
// We use this as the source for the future "import all sets"
// process instead of hardcoding set codes.
    public async Task<MtgJsonSetListResponse?> GetSetListAsync()
    {
        const string url = "https://mtgjson.com/api/v5/SetList.json";

        return await _httpClient.GetFromJsonAsync<MtgJsonSetListResponse>(url);
    }
}