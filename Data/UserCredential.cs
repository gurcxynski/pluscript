using System.ComponentModel.DataAnnotations;

namespace PluScript.Data;

public class UserCredential
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string Username { get; set; } = string.Empty;

	[Required]
	public string Password { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}