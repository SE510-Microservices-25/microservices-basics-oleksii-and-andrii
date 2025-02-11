namespace PollSystem.Entities;

public class Poll
{
	public Poll(int id, PollCreateDto pollCreateDto)
	{
		Id = id;
		Question = pollCreateDto.Question;
		Options = pollCreateDto.Options
			.Select((option, index) => new PollOption(index + 1, option))
			.ToList();
		ExpirationTime = pollCreateDto.ExpirationTime;
	}

	public int Id { get; init; }
	public string Question { get; set; }
	public List<PollOption> Options { get; set; }
	public DateTime ExpirationTime { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is not Poll other)
		{
			return false;
		}

		return Id == other.Id
			&& Question == other.Question
			&& Options.SequenceEqual(other.Options)
			&& ExpirationTime == other.ExpirationTime;
	}

	public override int GetHashCode()
	{
		return Id;
	}
}

public class PollCreateDto
{
	public string Question { get; init; }
	public List<PollOptionCreateDto> Options { get; init; } = new();
	public DateTime ExpirationTime { get; init; } = DateTime.UtcNow.AddDays(7);

	public override bool Equals(object? obj)
	{
		if (obj is not PollCreateDto other)
		{
			return false;
		}

		return Question == other.Question
			&& Options.SequenceEqual(other.Options)
			&& ExpirationTime == other.ExpirationTime;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Question, ExpirationTime);
	}
}
