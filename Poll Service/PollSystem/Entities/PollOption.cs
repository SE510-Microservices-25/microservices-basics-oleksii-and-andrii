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

	public override bool Equals(object? obj) => obj is PollOption pollOption && Id == pollOption.Id;
	public override int GetHashCode() => Id.GetHashCode();
}

public class PollOptionCreateDto
{
	public required string Text { get; init; }
}
