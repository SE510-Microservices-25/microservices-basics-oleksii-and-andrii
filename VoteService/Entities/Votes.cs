using System.ComponentModel.DataAnnotations;
using MediatR;

namespace PollSystem.Entities;

public interface IHasDomainEvents
{
    List<INotification> DomainEvents { get; }
}

public class Poll : IHasDomainEvents
{
    public Poll() { }

    public Poll(string question, List<PollOption> options, DateTime expirationDate)
    {
        Question = question;
        Options = options;
        ExpirationDate = expirationDate;
        DomainEvents.Add(new PollAddedEvent(this));
    }

    [Key] public int Id { get; init; }
    [Required, MaxLength(255)] public string Question { get; set; }
    public List<INotification> DomainEvents { get; } = new List<INotification>();
    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
    public DateTime ExpirationDate { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}

public class PollOption
{
    public PollOption() { }

    public PollOption(string text)
    {
        Text = text;
    }

    [Key] public int Id { get; init; }
    [Required, MaxLength(255)] public string Text { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}

public class PollAddedEvent(Poll poll) : INotification
{
    public Poll Poll { get; } = poll;
}