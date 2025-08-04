using Microsoft.EntityFrameworkCore;

namespace PluScript.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<UserCredential> UserCredentials { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	base.OnModelCreating(modelBuilder);

	modelBuilder.Entity<UserCredential>()
		.HasIndex(u => u.Username)
		.IsUnique();
}
}