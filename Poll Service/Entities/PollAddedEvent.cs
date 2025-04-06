using MediatR;

namespace PollSystem.Entities;

public class PollAddedEvent(Poll poll) : INotification
{
    public Poll Poll { get; } = poll;
}