using Microsoft.EntityFrameworkCore;
using PluScript.Data;

namespace PluScript.Services;

public class UserCredentialsService(ApplicationDbContext context)
{
	private readonly ApplicationDbContext _context = context;

	public async Task StoreCredentialsAsync(string username, string password)
	{
		var existingCredential = await _context.UserCredentials
			.FirstOrDefaultAsync(u => u.Username == username);

		if (existingCredential != null)
		{
			existingCredential.Password = password;
			existingCredential.CreatedAt = DateTime.UtcNow;
		}
		else
		{
			_context.UserCredentials.Add(new UserCredential
			{
				Username = username,
				Password = password
			});
		}

		await _context.SaveChangesAsync();
	}

	public async Task RemoveCredentialsAsync(string username)
	{
		var credential = await _context.UserCredentials
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

		if (credential != null)
		{
			_context.UserCredentials.Remove(credential);
			await _context.SaveChangesAsync();
		}
	}

	public async Task<IEnumerable<StoredUserCredentials>> GetAllStoredCredentialsAsync()
	{
		var credentials = await _context.UserCredentials.ToListAsync();
		return credentials.Select(c => new StoredUserCredentials
		{
			Username = c.Username,
			Password = c.Password,
			StoredAt = c.CreatedAt
		});
	}

	public async Task<StoredUserCredentials?> GetCredentialsAsync(string username)
	{
		var credential = await _context.UserCredentials
			.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

		if (credential != null)
		{
			return new StoredUserCredentials
			{
				Username = credential.Username,
				Password = credential.Password,
				StoredAt = credential.CreatedAt
			};
		}

		return null;
	}

	public async Task<bool> HasCredentialsAsync(string username)
	{
		return await _context.UserCredentials
			.AnyAsync(u => u.Username.ToLower() == username.ToLower());
	}

	public async Task<int> GetStoredCredentialsCountAsync()
	{
		return await _context.UserCredentials.CountAsync();
	}
}

public class StoredUserCredentials
{
	public string Username { get; set; } = "";
	public string Password { get; set; } = "";
	public DateTime StoredAt { get; set; }
}