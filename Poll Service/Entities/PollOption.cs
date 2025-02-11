namespace PollSystem.Entities;

public class PollOption
{
	public PollOption(int id, PollOptionCreateDto pollOptionCreateDto)
	{
		Id = id;
		Text = pollOptionCreateDto.Text;
	}

	public int Id { get; init; }
	public string Text { get; set; }

	public override bool Equals(object? obj)
	{
		if (obj is not PollOption other)
		{
			return false;
		}

		return Id == other.Id && Text == other.Text;
	}

	public override int GetHashCode()
	{
		return Id;
	}
}

public class PollOptionCreateDto
{
	public required string Text { get; init; }

	public override bool Equals(object? obj)
	{
		if (obj is not PollOptionCreateDto other)
		{
			return false;
		}

		return Text == other.Text;
	}

	public override int GetHashCode()
	{
		return Text.GetHashCode();
	}
}
