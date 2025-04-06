using System.Text.Json;
using MassTransit;
using PollSystem.Data;
using PollSystem.Entities;

namespace PollSystem.Services;

public class OutboxProcessor : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public OutboxProcessor(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using IServiceScope scope = _serviceProvider.CreateScope();
		AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		IPublishEndpoint publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

		while (!stoppingToken.IsCancellationRequested)
		{
			List<OutboxMessage> outboxMessages = dbContext.OutboxMessages
				.Where(message => !message.Processed)
				.ToList();

			foreach (OutboxMessage message in outboxMessages)
			{
				Type? eventType = Type.GetType(message.Type);
				if (eventType == null) continue;

				object? eventData = JsonSerializer.Deserialize(message.Payload, eventType);
				if (eventData == null) continue;

				await publishEndpoint.Publish(eventData, stoppingToken);

				message.Processed = true;
				await dbContext.SaveChangesAsync(stoppingToken);
			}

			await Task.Delay(5000, stoppingToken); // Run every 5 seconds
		}
	}
}
