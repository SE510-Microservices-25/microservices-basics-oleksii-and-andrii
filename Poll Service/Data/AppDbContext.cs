using Microsoft.EntityFrameworkCore;
using PollSystem.Entities;

namespace PollSystem.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Poll> Polls { get; set; }
	public DbSet<PollOption> PollOptions { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Poll>()
			.HasMany(poll => poll.Options)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);
	}
}
