using MassTransit;
using VoteSystem.Models;

namespace VoteSystem.Consumer;

public class VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger) : IConsumer<Vote>
{
    public Task Consume(ConsumeContext<Vote> context)
    {
        logger.LogInformation($"Vote received: Id: {context.Message.Id}, Date: {context.Message.CreatedAt}");
        return Task.CompletedTask;
    }
}