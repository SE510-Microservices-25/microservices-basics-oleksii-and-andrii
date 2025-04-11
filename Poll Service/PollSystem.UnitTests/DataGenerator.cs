using PollSystem.Entities;

namespace PollSystem.UnitTests;

public static class DataGenerator
{
	public static List<Poll> GeneratePolls(int count = 10)
	{
		List<Poll> polls = new List<Poll>();

		for (int i = 1; i <= count; i++)
		{
			polls.Add(new Poll()
			{
				Question = $"Question #{i}",
				Options = GeneratePollOptions(),
				ExpirationDate = DateTime.Now.AddDays(i)
			});
		}

		return polls;
	}

	public static List<PollOption> GeneratePollOptions(int count = 2)
	{
		List<PollOption> pollOptions = new List<PollOption>();

		for (int i = 1; i <= count; i++)
		{
			pollOptions.Add(new PollOption()
			{
				Text = $"Option #{i}"
			});
		}

		return pollOptions;
	}
}
