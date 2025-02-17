using System.ComponentModel.DataAnnotations;

namespace PollSystem.Entities;

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

public class PollOptionCreateDto
{
	public required string Text { get; init; }
}
