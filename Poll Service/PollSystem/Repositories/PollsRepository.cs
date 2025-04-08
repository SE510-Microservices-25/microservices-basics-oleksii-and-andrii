using Microsoft.EntityFrameworkCore;
using PollSystem.Data;
using PollSystem.Entities;

namespace PollSystem.Repositories;

public class PollsRepository(AppDbContext context)
{
	public async Task<List<Poll>> GetAllPollsAsync(CancellationToken cancellationToken)
	{
		return await context.Polls.Include(poll => poll.Options).ToListAsync(cancellationToken);
	}

	public async Task<Poll?> GetPollByIdAsync(int id, CancellationToken cancellationToken)
	{
		return await context.Polls
			.Include(poll => poll.Options)
			.FirstOrDefaultAsync(poll => poll.Id == id, cancellationToken);
	}

	public async Task<Poll?> CreatePollAsync(Poll poll, CancellationToken cancellationToken)
	{
		try
		{
			await context.AddAsync(poll, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
			return poll;
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			return null;
		}
	}

	public async Task<bool> AddOptionsAsync(List<PollOption> option, CancellationToken cancellationToken)
	{
		try
		{
			await context.PollOptions.AddRangeAsync(option, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
			return true;
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			return false;
		}
	}

	public async Task<Poll?> UpdatePollAsync(int id, PollUpdateDto poll, CancellationToken cancellationToken)
	{
		// Fetch the existing poll from the database, including its options
		Poll? existingPoll = await context.Polls
			.Include(p => p.Options)
			.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

		if (existingPoll == null)
		{
			return null;
		}

		// Update poll fields
		existingPoll.Question = poll.Question;
		existingPoll.ExpirationDate = poll.ExpirationDate;
		existingPoll.UpdatedOn = DateTime.UtcNow;

		// Update PollOptions: Remove old ones, add new ones
		context.PollOptions.RemoveRange(existingPoll.Options);

		List<PollOption> newOptions = poll.Options
			.Select(option => new PollOption(option.Text))
			.ToList();

		await context.PollOptions.AddRangeAsync(newOptions, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		existingPoll.Options = newOptions;

		await context.SaveChangesAsync(cancellationToken);

		return existingPoll;
	}

	public async Task<bool> DeletePollAsync(int id, CancellationToken cancellationToken)
	{
		Poll? poll = await context.Polls.FindAsync(id, cancellationToken);
		if (poll == null)
		{
			return false;
		}

		try
		{
			context.Polls.Remove(poll);
			await context.SaveChangesAsync(cancellationToken);
			return true;
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			return false;
		}
	}
}
