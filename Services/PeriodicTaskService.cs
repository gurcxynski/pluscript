using PluScript.Script;

namespace PluScript.Services;

public class PeriodicTaskService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	private readonly TimeSpan _period = TimeSpan.FromHours(1);

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await UserSessions.ExecuteTaskForAllUsers(serviceScopeFactory);
			await Task.Delay(_period, stoppingToken);
		}
	}

}