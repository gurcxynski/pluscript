using System;

namespace PluScript.Services;

public class AuthenticationService
{
    private readonly UserCredentialsService _userCredentialsService;
    
    public event Action? OnAuthenticationStateChanged;

    public string? Username { get; private set; }

    public AuthenticationService(UserCredentialsService userCredentialsService)
    {
        _userCredentialsService = userCredentialsService;
    }

    public async Task LoginAsync(string username, string password)
    {
        Username = username;
        
        // Store credentials in SQLite database
        await _userCredentialsService.StoreCredentialsAsync(username, password);
        
        OnAuthenticationStateChanged?.Invoke();
    }

    public async Task<bool> RestoreSessionAsync(string username)
    {
        var credentials = await _userCredentialsService.GetCredentialsAsync(username);
        if (credentials != null)
        {
            Username = username;
            OnAuthenticationStateChanged?.Invoke();
            return true;
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        if (!string.IsNullOrEmpty(Username))
        {
            await _userCredentialsService.RemoveCredentialsAsync(Username);
            Username = null;
            OnAuthenticationStateChanged?.Invoke();
        }
    }
}
