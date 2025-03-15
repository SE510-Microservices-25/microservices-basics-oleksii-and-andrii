using MassTransit;
using Microsoft.Extensions.Logging;
using VoteSystem.Contracts;

namespace VoteSystem.Consumer;

public class VoteCreatedConsumer : IConsumer<VoteCreated>
{
    private readonly ILogger<VoteCreatedConsumer> _logger;

    public VoteCreatedConsumer(ILogger<VoteCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<VoteCreated> context)
    {
        _logger.LogInformation($"Vote received: Id: {context.Message.Id}, Date: {context.Message.Date}");
        return Task.CompletedTask;
    }
}