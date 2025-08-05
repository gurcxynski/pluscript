namespace PluScript.Services;

public class AuthService
{
    private readonly string _adminPassword;
    
    public event Action? OnAuthStateChanged;

    public bool IsAuthenticated { get; private set; }

    public AuthService()
    {
        _adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!;
    }
    
    public bool Login(string password)
    {
        if (password == _adminPassword)
        {
            IsAuthenticated = true;
            OnAuthStateChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        OnAuthStateChanged?.Invoke();
    }
}
