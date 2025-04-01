using MassTransit;
using Microsoft.Extensions.Logging;
using PollSystem.Entities;

namespace PollSystem.Consumers;

public class PollCreatedConsumer : IConsumer<PollCreateDto>
{
	private readonly ILogger<PollCreatedConsumer> _logger;

	public PollCreatedConsumer(ILogger<PollCreatedConsumer> logger)
	{
		_logger = logger;
	}

	public Task Consume(ConsumeContext<PollCreateDto> context)
	{
		_logger.LogInformation($"Poll received: \"{context.Message.Question}\" till {context.Message.ExpirationDate} ({string.Join(", ", context.Message.Options.Select(it => it.Text))})");
		return Task.CompletedTask;
	}
}
