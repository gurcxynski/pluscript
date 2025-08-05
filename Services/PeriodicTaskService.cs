using PluScript.Script;

namespace PluScript.Services;

public class PeriodicTaskService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly TimeSpan _period = TimeSpan.FromHours(1);

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
		var loggingService = scope.ServiceProvider.GetRequiredService<LoggingService>();

		var storedCredentials = await userCredentialsService.GetAllStoredCredentialsAsync();

		foreach (var credentials in storedCredentials)
		{
			await RunTestsIfNeeded(credentials, loggingService);
		}
	}
	
	private static async Task RunTestsIfNeeded(StoredUserCredentials credentials, LoggingService loggingService)
	{
		var (token, userId, username) = await Login.LoginAsync(credentials.Username, credentials.Password);
		loggingService.Log($"Checking scores for user: {username} (ID: {userId})", "PeriodicTask");
		if (GetScores.GetTotalScore(token).GetAwaiter().GetResult() == 100) {
			loggingService.Log($"User {username} (ID: {userId}) already has full score, skipping tests.", "PeriodicTask");
			return;
		}
		await RunTestSession(token, userId, username, loggingService);
	}
	
	private static async Task RunTestSession(string token, int userId, string username, LoggingService loggingService)
	{
		loggingService.Log($"Running test session for user: {username} (ID: {userId})", "PeriodicTask");
		SessionHandler sessionHandler = new();
		await Task.Run(() => sessionHandler.RunTests(token, userId).GetAwaiter().GetResult());
		loggingService.Log($"Test session completed for user: {username} (ID: {userId})", "PeriodicTask");
	}
}