using PluScript.Script;

namespace PluScript.Services;

public class PeriodicTaskService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly TimeSpan _period = TimeSpan.FromMinutes(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await ExecuteTaskForAllUsers();
			await Task.Delay(_period, stoppingToken);
		}
	}

	private async Task ExecuteTaskForAllUsers()
	{
		using var scope = _serviceScopeFactory.CreateScope();
		var userCredentialsService = scope.ServiceProvider.GetRequiredService<UserCredentialsService>();

		var storedCredentials = await userCredentialsService.GetAllStoredCredentialsAsync();

		foreach (var credentials in storedCredentials)
		{
			RunTestsIfNeeded(credentials);
		}
	}
	private static async Task RunTestsIfNeeded(StoredUserCredentials credentials)
	{
		var (token, userId, username) = await Login.LoginAsync(credentials.Username, credentials.Password);
		Console.WriteLine($"Checking scores for user: {username} (ID: {userId}) with token: {token}");
		if (GetScores.GetTotalScore(token).GetAwaiter().GetResult() == 100) return;
		RunTestSession(token, userId, username);
	}
	private static async Task RunTestSession(string token, int userId, string username)
	{
		Console.WriteLine($"Running test session for user: {username} (ID: {userId}) with token: {token}");
		SessionHandler sessionHandler = new();
		sessionHandler.RunTests(token, userId).GetAwaiter().GetResult();
		Console.WriteLine($"Test session completed for user: {username} (ID: {userId})");
	}
}