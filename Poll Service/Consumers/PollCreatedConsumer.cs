using MassTransit;
using Microsoft.Extensions.Logging;
using PollSystem.Entities;

namespace PollSystem.Consumers;

public class PollCreatedConsumer(ILogger<PollCreatedConsumer> logger) : IConsumer<PollCreateDto>
{
	public Task Consume(ConsumeContext<PollCreateDto> context)
	{
		logger.LogInformation($"Poll received: \"{context.Message.Question}\" till {context.Message.ExpirationDate} ({string.Join(", ", context.Message.Options.Select(it => it.Text))})");
		return Task.CompletedTask;
	}
}
