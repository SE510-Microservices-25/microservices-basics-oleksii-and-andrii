using MassTransit;

namespace VoteSystem.Services;

public class RabbitMqService
{
    private readonly IBus _bus;

    public RabbitMqService(IBus bus)
    {
        _bus = bus;
    }

    public async Task SendMessage<T>(T message) where T : class
    {
        await _bus.Publish(message);
    }
}