using System.Text;
using System.Text.Json;

namespace PluScript.Script;

public class PluRetriever
{
	private readonly Dictionary<string, int> cache = [];
	private readonly HttpClient httpClient = new();

	public async Task<int> GetPluNumber(string token, string phrase)
	{
		if (cache.TryGetValue(phrase, out int value))
		{
			return value;
		}

		string url = "https://easy-plu.knowledge-hero.com/api/plu/product/search";
		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		var payload = new { search = phrase };
		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		var list = data.GetProperty("data").GetProperty("products").GetProperty("data");
		if (list.GetArrayLength() == 0)
		{
			throw new ArgumentException($"No products found for {phrase}");
		}

		var best = list[0];
		var plu = best.GetProperty("plu_number").GetInt32();
		cache[phrase] = plu;
		return plu;
	}
}
