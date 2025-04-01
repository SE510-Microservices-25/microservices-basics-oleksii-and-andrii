using System.ComponentModel.DataAnnotations;

namespace PollSystem.Entities;

public class Poll
{
	public Poll() { }

	public Poll(string question, List<PollOption> options, DateTime expirationDate)
	{
		Question = question;
		Options = options;
		ExpirationDate = expirationDate;
	}

	[Key] public int Id { get; init; }
	[Required, MaxLength(255)] public string Question { get; set; }
	public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
	public DateTime ExpirationDate { get; set; }

	public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
}

public class PollCreateDto
{
	public string Question { get; init; } = string.Empty;
	public List<PollOptionCreateDto> Options { get; init; } = new();
	public DateTime ExpirationDate { get; init; } = DateTime.UtcNow.AddDays(3);
}

public class PollUpdateDto
{
	public string Question { get; init; } = string.Empty;
	public List<PollOptionCreateDto> Options { get; init; } = new();
	public DateTime ExpirationDate { get; init; } = DateTime.UtcNow.AddDays(3);
}
