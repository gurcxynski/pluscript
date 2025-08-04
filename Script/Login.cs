using System.Text;
using System.Text.Json;

namespace PluScript.Script;

	public class Login
{
	private static readonly HttpClient httpClient = new HttpClient();

	public static async Task<(string Token, int UserId, string Username)> LoginWithUsername(string name, string password)
	{
		string url = "https://easy-plu.knowledge-hero.com/api/plu/login";

		var payload = new
		{
			name,
			password
		};

		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();
		Console.WriteLine("Logowanie zakończone sukcesem.");
		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		string token = data.GetProperty("api_token").GetString() ?? "";
		int userId = data.GetProperty("user").GetProperty("id").GetInt32();
		string userName = data.GetProperty("user").GetProperty("name").GetString() ?? "";

		return (token, userId, userName);
	}

	public static async Task<(string Token, int UserId, string Username)> LoginWithEmail(string email, string password)
	{
		string url = "https://easy-plu.knowledge-hero.com/api/plu/login";

		var payload = new
		{
			email,
			password
		};

		string jsonPayload = JsonSerializer.Serialize(payload);
		var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync(url, content);
		response.EnsureSuccessStatusCode();

		string responseBody = await response.Content.ReadAsStringAsync();
		var data = JsonSerializer.Deserialize<JsonElement>(responseBody);

		string token = data.GetProperty("api_token").GetString() ?? "";
		int userId = data.GetProperty("user").GetProperty("id").GetInt32();
		string userName = data.GetProperty("user").GetProperty("name").GetString() ?? "";

		return (token, userId, userName);
	}

	public static async Task<(string Token, int UserId, string Username)> LoginAsync(string name, string password)
	{
		if (name.Contains('@'))
		{
			return await LoginWithEmail(name, password);
		}
		else
		{
			return await LoginWithUsername(name, password);
		}
	}
}
