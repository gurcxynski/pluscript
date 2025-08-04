using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace PluScript.Script;

public class CheckPluFromPython
{
	private static readonly Dictionary<string, int> cache = new Dictionary<string, int>();
	private static readonly HttpClient httpClient = new HttpClient();

	public static async Task<int> GetPluNumber(string token, string phrase)
	{
		if (cache.ContainsKey(phrase))
		{
			return cache[phrase];
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
