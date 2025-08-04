using System.Text;
using System.Text.Json;

namespace PluScript.Script;

public class SessionHandler()
{
	private readonly HttpClient httpClient = new();
	private string token;
	private int userId;
	private string sessionId;
	private readonly PluRetriever pluRetriever = new();

	public async Task InitializeSession(string token, int userId, int itemCount)
	{
		string url = "https://easy-plu.knowledge-hero.com/api/plu/plu-learn/create-new-session";

		var payload = new
		{
			product_category_id = (int?)null,
			count_selection = itemCount,
			user_id = userId,
			language_id = 20,
			execution_type = 3,
			execution_subtype = 0,
			ean_active = false,
			top_article_active = false,
			plu_current_count = 1,
			attribute_group_id = (int?)null,
			new_plu = (string?)null
		};

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");
		this.token = token;
		this.userId = userId;

		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		sessionId = data.GetProperty("data").GetProperty("session_id").GetString();
	}

	public async Task<List<(string Title, int Id, string ItemId)>> GetExecutionItems()
	{
		string url = $"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{sessionId}/execution-items";

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		var content = new StringContent("{}", Encoding.UTF8, "application/json");
		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		var ret = new List<(string, int, string)>();
		var items = data.GetProperty("data").GetProperty("items");

		foreach (var item in items.EnumerateArray())
		{
			var pluObj = item.GetProperty("pluNumber");
			string title;

			if (pluObj.TryGetProperty("title", out var titleProp) && titleProp.ValueKind != JsonValueKind.Null)
			{
				title = titleProp.GetString();
			}
			else
			{
				var translations = pluObj.GetProperty("translations");
				var firstTranslation = translations.EnumerateObject().GetEnumerator();
				firstTranslation.MoveNext();
				title = firstTranslation.Current.Value.GetProperty("title").GetString();
			}

			var pluId = pluObj.GetProperty("id").GetInt32();
			var itemId = item.GetProperty("id").GetString();

			ret.Add((title, pluId, itemId));
		}

		return ret;
	}
	public async Task StartExecution()
	{
		string url = $"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{sessionId}/start-execution";

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		var content = new StringContent("{}", Encoding.UTF8, "application/json");
		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();
	}
	public async Task SendAnswer(string taskId, int pluNumber, int pluId)
	{
		string url = $"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{taskId}/update";

		var payload = new
		{
			execution_type = 3,
			given_plu_number = pluNumber,
			plu_number_id = pluId,
			answer = new { correct = true }
		};

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PutAsync(url, content);
		response.EnsureSuccessStatusCode();
	}
	public async Task<(int UserScore, int MaxScore)> GetResult()
	{
		string url = $"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{sessionId}/result";

		var payload = new { user_id = userId };

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");

		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		int result = data.GetProperty("data").GetProperty("result").GetProperty("total_user_points").GetInt32();
		int maxResult = data.GetProperty("data").GetProperty("result").GetProperty("max_points").GetInt32();

		return (result, maxResult);
	}
	public async Task RunTests(string token, int userId)
	{
		do
		{
			var session = new SessionHandler();
			await session.InitializeSession(token, userId, 20);
			var items = await session.GetExecutionItems();
			await session.StartExecution();

			Console.WriteLine($"Rozpoczęto sesję. Wybrano {items.Count} produktów.");

			foreach (var item in items)
			{
				var plu = await pluRetriever.GetPluNumber(token, item.Title);
				Console.WriteLine($"Dla {item.Title} znaleziono PLU {plu}");
				await session.SendAnswer(item.ItemId, plu, item.Id);
			}
			var (userScore, maxScore) = await session.GetResult();
			Console.WriteLine($"Zakończono sesję. Wynik to {userScore}/{maxScore} pkt.");
		} while (await GetScores.GetTotalScore(token) < 100);
	}
}