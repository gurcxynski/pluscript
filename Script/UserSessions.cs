using PluScript.Script;
using PluScript.Services;

abstract class UserSessions
{
	public static async Task ExecuteTaskForAllUsers(IServiceScopeFactory serviceScopeFactory, bool force = false)
	{
		using var scope = serviceScopeFactory.CreateScope();
		var userCredentialsService = scope.ServiceProvider.GetRequiredService<UserCredentialsService>();
		var loggingService = scope.ServiceProvider.GetRequiredService<LoggingService>();

		var storedCredentials = await userCredentialsService.GetAllStoredCredentialsAsync();

		var tasks = storedCredentials.Select(credentials => 
		    RunTests(credentials, loggingService, force)
		);
		
		await Task.WhenAll(tasks);
	}
	private static async Task RunTests(StoredUserCredentials credentials, LoggingService loggingService, bool force = false)
	{
		var (token, userId, username) = await Login.LoginAsync(credentials.Username, credentials.Password);
		loggingService.Log($"Checking scores for user: {username} (ID: {userId})", "PeriodicTask");
		if (!force && GetScores.GetTotalScore(token).GetAwaiter().GetResult() == 100) {
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