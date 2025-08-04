using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PluScript.Script;

public class GetScores
{
	private static readonly HttpClient httpClient = new HttpClient();

	public static async Task<double> GetTotalScore(string token)
	{
		string url = "https://easy-plu.knowledge-hero.com/api/plu/knowledge/user";
		
		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		var response = await httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);
		
		return data.GetProperty("data").GetDouble();
	}

	public static async Task<List<(string Result, string Category)>> GetScoresByCategory(string token)
	{
		string url = "https://easy-plu.knowledge-hero.com/api/plu/knowledge/user/product-groups-results";
		
		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		var response = await httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		var scores = new List<(string, string)>();
		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);
		var productCategories = data.GetProperty("data").GetProperty("productCategories");
		
		for (int i = 0; i < 4; i++)
		{
			var categoryData = productCategories[i];
			string result = categoryData.GetProperty("result").GetString();
			string categoryName = categoryData.GetProperty("plu_product_category").GetProperty("name").GetString();
			scores.Add((result, categoryName));
		}
		
		return scores;
	}
}
