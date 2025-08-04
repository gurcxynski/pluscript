using Microsoft.Extensions.DependencyInjection;

namespace PluScript.Services
{
    public class PeriodicTaskService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PeriodicTaskService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(10);

        public PeriodicTaskService(IServiceScopeFactory serviceScopeFactory, ILogger<PeriodicTaskService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Periodic Task Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExecuteTaskForAllUsers();
                    await Task.Delay(_period, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Periodic Task Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Periodic Task Service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute before retrying
                }
            }
        }

        private async Task ExecuteTaskForAllUsers()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var userCredentialsService = scope.ServiceProvider.GetRequiredService<UserCredentialsService>();
            
            var storedCredentials = await userCredentialsService.GetAllStoredCredentialsAsync();
            
            _logger.LogInformation($"Executing periodic task for {storedCredentials.Count()} stored users");

            foreach (var credentials in storedCredentials)
            {
                try
                {
                    await Foo(credentials);
                    _logger.LogDebug($"Executed Foo() for user {credentials.Username}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error executing Foo() for user {credentials.Username}");
                }
            }
        }

        /// <summary>
        /// This is the Foo() function that gets executed every 10 minutes for each stored user.
        /// Modify this method to implement your specific business logic.
        /// </summary>
        /// <param name="credentials">The stored user credentials</param>
        private async Task Foo(StoredUserCredentials credentials)
        {
            // TODO: Implement your specific logic here
            // You have access to:
            // - credentials.Username
            // - credentials.Password
            // - credentials.StoredAt

            _logger.LogInformation($"Executing Foo() for user: {credentials.Username}");

            // Example: You could authenticate with the API using the stored credentials
            // and then perform some operation
            
            // For now, just log the execution
            await Task.Delay(100); // Simulate some work
            
            // Example of what you might do:
            /*
            try
            {
                // Login with the stored credentials to get a fresh token
                var loginResult = await PluScript.Login.LoginAsync(credentials.Username, credentials.Password);
                
                if (!string.IsNullOrEmpty(loginResult.Token))
                {
                    // Use the token to make API calls
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);
                    
                    // Perform your specific task
                    var response = await httpClient.GetAsync("https://your-api-endpoint.com/user-task");
                    // Process the response...
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute task for user {credentials.Username}");
            }
            */
        }
    }
}
