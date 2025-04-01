using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollSystem.Data;
using PollSystem.Entities;

namespace PollSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class PollsController : ControllerBase
{
	private readonly AppDbContext _context;

	public PollsController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<Poll>>> GetPollsAsync()
	{
		return await _context.Polls.Include(poll => poll.Options).ToListAsync();
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Poll>> GetPollByIdAsync(int id)
	{
		Poll? poll = await _context.Polls
			.Include(poll => poll.Options)
			.FirstOrDefaultAsync(poll => poll.Id == id);

		if (poll == null)
		{
			return NotFound();
		}

		return poll;
	}

	[HttpPost]
	public async Task<ActionResult<Poll>> CreatePollAsync(PollCreateDto poll)
	{
		// Ensure ExpirationDate is at least 7 days in the future
		DateTime expirationDate = poll.ExpirationDate == default
			? DateTime.UtcNow.AddDays(7)
			: poll.ExpirationDate;

		if (expirationDate < DateTime.UtcNow)
		{
			return BadRequest("Expiration date cannot be in the past");
		}

		// Add poll options
		List<PollOption> pollOptions = poll.Options
			.Select(option => new PollOption(option.Text))
			.ToList();
		await _context.PollOptions.AddRangeAsync(pollOptions);
		await _context.SaveChangesAsync();

		// Add poll itself
		Poll newPoll = new Poll(
			poll.Question,
			pollOptions,
			expirationDate
		);
		await _context.AddAsync(newPoll);
		await _context.SaveChangesAsync();

		string actionName = nameof(GetPollByIdAsync);
		// TODO: fix System.InvalidOperationException: No route matches the supplied values.
		return CreatedAtAction(
			actionName,
			new { id = newPoll.Id },
			newPoll
		);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdatePollAsync(int id, [FromBody] PollUpdateDto pollDto)
	{
		if (pollDto.ExpirationDate < DateTime.UtcNow)
		{
			return BadRequest("Expiration date cannot be in the past");
		}

		// Fetch the existing poll from the database, including its options
		Poll? existingPoll = await _context.Polls
			.Include(p => p.Options)
			.FirstOrDefaultAsync(p => p.Id == id);

		if (existingPoll == null)
		{
			return NotFound();
		}

		// Update poll fields
		existingPoll.Question = pollDto.Question;
		existingPoll.ExpirationDate = pollDto.ExpirationDate;
		existingPoll.UpdatedOn = DateTime.UtcNow;

		// Update PollOptions: Remove old ones, add new ones
		_context.PollOptions.RemoveRange(existingPoll.Options);

		List<PollOption> newOptions = pollDto.Options
			.Select(option => new PollOption(option.Text))
			.ToList();

		await _context.PollOptions.AddRangeAsync(newOptions);
		await _context.SaveChangesAsync();

		existingPoll.Options = newOptions;

		await _context.SaveChangesAsync();

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult<Poll>> DeletePollAsync(int id)
	{
		Poll? poll = await _context.Polls.FindAsync(id);
		if (poll == null)
		{
			return NotFound();
		}

		_context.Polls.Remove(poll);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}
