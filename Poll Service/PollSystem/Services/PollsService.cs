using MassTransit;
using PollSystem.Entities;
using PollSystem.Repositories;

namespace PollSystem.Services;

public class PollsService(PollsRepository repository, IBus bus)
{
	public async Task<List<Poll>> GetAllPollsAsync(CancellationToken cancellationToken)
	{
		List<Poll> polls = await repository.GetAllPollsAsync(cancellationToken);
		return polls;
	}

	public async Task<Poll?> GetPollByIdAsync(int id, CancellationToken cancellationToken)
	{
		Poll? poll = await repository.GetPollByIdAsync(id, cancellationToken);
		return poll;
	}

	public async Task<Poll?> CreatePollAsync(PollCreateDto poll, CancellationToken cancellationToken)
	{
		// Ensure ExpirationDate is at least 7 days in the future
		DateTime expirationDate = poll.ExpirationDate == default
			? DateTime.UtcNow.AddDays(7)
			: poll.ExpirationDate;

		if (expirationDate < DateTime.UtcNow)
		{
			throw new Exception("Expiration date cannot be in the past");
		}

		// Add poll options
		List<PollOption> pollOptions = poll.Options
			.Select(option => new PollOption(option.Text))
			.ToList();
		if (!await repository.AddOptionsAsync(pollOptions, cancellationToken))
		{
			throw new Exception("Failed to add poll options");
		}

		// Add poll itself
		Poll newPoll = new Poll(
			poll.Question,
			pollOptions,
			expirationDate
		);
		await repository.CreatePollAsync(newPoll, cancellationToken);

		return newPoll;
	}

	public async Task<Poll?> UpdatePollAsync(int id, PollUpdateDto poll, CancellationToken cancellationToken)
	{
		try
		{
			return await repository.UpdatePollAsync(id, poll, cancellationToken);
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw new Exception($"Failed to update poll");
		}
	}

	public async Task DeletePollAsync(int id, CancellationToken cancellationToken)
	{
		if (!await repository.DeletePollAsync(id, cancellationToken))
		{
			throw new Exception("Failed to delete poll");
		}
	}
}
