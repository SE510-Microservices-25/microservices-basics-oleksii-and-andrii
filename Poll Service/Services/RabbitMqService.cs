using MassTransit;

namespace PollSystem.Services;

public class RabbitMqService(IBus bus)
{
	public async Task SendMessage<T>(T message) where T : class
	{
		await bus.Publish(message);
	}
}
