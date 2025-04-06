namespace PollSystem.Entities;

public class OutboxMessage
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Event type
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// Serialized JSON
	/// </summary>
	public string Payload { get; set; }

	/// <summary>
	/// Marks if published
	/// </summary>
	public bool Processed { get; set; }
}
