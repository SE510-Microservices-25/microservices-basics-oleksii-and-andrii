namespace PollSystem.Entities;

public interface IHasDomainEvents
{
	List<DomainEvent> DomainEvents { get; }
}

public abstract class DomainEvent {}
