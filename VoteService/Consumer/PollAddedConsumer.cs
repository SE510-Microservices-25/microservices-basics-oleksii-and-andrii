using MassTransit;
using PollSystem.Entities;

namespace VoteSystem.Consumer;

public class PollAddedConsumer(ILogger<PollAddedConsumer> logger) : IConsumer<PollAddedEvent>
{
    public Task Consume(ConsumeContext<PollAddedEvent> context)
    {
        logger.LogInformation(
            $"Poll received: Created at: {context.Message.Poll.CreatedOn}, Question: {context.Message.Poll.Question}, Expiration Date: {context.Message.Poll.ExpirationDate}, Options: {string.Join(", ", context.Message.Poll.Options.Select(it => it.Text))}");
        return Task.CompletedTask;
    }
}