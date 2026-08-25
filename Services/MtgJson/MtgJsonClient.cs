namespace MTGWantList.Services.MtgJson;

public class MtgJsonClient
{
    // HttpClient is provided by ASP.NET through dependency injection.
    // We use it to make HTTP requests to the MTGJSON API.
    private readonly HttpClient _httpClient;

    public MtgJsonClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Downloads the raw JSON data for a specific Magic set.
    // Example:
    // "FDN" -> https://mtgjson.com/api/v5/FDN.json
    public async Task<string> GetSetAsync(string setCode)
    {
        // MTGJSON set codes are conventionally uppercase,
        // so we normalise the input before building the URL.
        string url =
            $"https://mtgjson.com/api/v5/{setCode.ToUpperInvariant()}.json";

        // Download the response body and return it as raw JSON text.
        return await _httpClient.GetStringAsync(url);
    }
}