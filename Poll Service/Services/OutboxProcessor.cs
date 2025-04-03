using System.Text.Json;
using MassTransit;
using PollSystem.Data;

namespace PollSystem.Services;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPublishEndpoint _publishEndpoint;

    public OutboxProcessor(IServiceProvider serviceProvider, IPublishEndpoint publishEndpoint)
    {
        _serviceProvider = serviceProvider;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var outboxMessages = dbContext.OutboxMessages
                .Where(m => !m.Processed)
                .ToList();

            foreach (var message in outboxMessages)
            {
                var eventType = Type.GetType(message.Type);
                if (eventType == null) continue;

                var eventData = JsonSerializer.Deserialize(message.Payload, eventType);
                if (eventData == null) continue;

                await _publishEndpoint.Publish(eventData, stoppingToken);

                message.Processed = true;
                await dbContext.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(5000, stoppingToken); // Runs every 5 seconds
        }
    }
}