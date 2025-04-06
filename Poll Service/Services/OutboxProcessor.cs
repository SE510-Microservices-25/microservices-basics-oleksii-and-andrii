using System.Text.Json;
using MassTransit;
using MassTransit.Serialization;
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
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var outboxMessages = dbContext.OutboxMessages
                .Where(m => !m.Processed)
                .ToList();

            foreach (var message in outboxMessages)
            {
                var eventType = Type.GetType(message.Type);
                if (eventType == null) continue;

                var eventData = JsonSerializer.Deserialize(message.Payload, eventType);
                if (eventData == null) continue;
                var newEventData = eventData.Transform<PollAddedEvent>(new JsonSerializerOptions());

                await publishEndpoint.Publish(newEventData, stoppingToken);

                message.Processed = true;
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}