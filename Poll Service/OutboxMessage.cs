namespace PollSystem;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty; // Event Type
    public string Payload { get; set; } = string.Empty; // Serialized JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false; // Marks if published
}