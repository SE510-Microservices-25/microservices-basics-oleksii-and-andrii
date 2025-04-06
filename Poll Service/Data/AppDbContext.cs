using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PollSystem.Entities;

namespace PollSystem.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<Poll> Polls { get; set; }
	public DbSet<PollOption> PollOptions { get; set; }

	public DbSet<OutboxMessage> OutboxMessages { get; set; }

	public override int SaveChanges()
	{
		ProcessOutboxMessages();
		return base.SaveChanges();
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		ProcessOutboxMessages();
		return await base.SaveChangesAsync(cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Poll>()
			.HasMany(poll => poll.Options)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);
	}

	private void ProcessOutboxMessages()
	{
		List<DomainEvent> domainEvents = ChangeTracker.Entries<IHasDomainEvents>()
			.SelectMany(e => e.Entity.DomainEvents)
			.ToList();

		foreach (var domainEvent in domainEvents)
		{
			var outboxMessage = new OutboxMessage
			{
				Type = domainEvent.GetType().FullName!,
				Payload = JsonSerializer.Serialize(domainEvent),
				CreatedAt = DateTime.UtcNow,
				Processed = false
			};

			OutboxMessages.Add(outboxMessage);
		}
	}
}
