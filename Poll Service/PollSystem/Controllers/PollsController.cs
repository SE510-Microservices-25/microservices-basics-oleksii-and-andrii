using MediatR;
using Microsoft.AspNetCore.Mvc;
using PollSystem.Commands;
using PollSystem.Entities;
using PollSystem.Queries;

namespace PollSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class PollsController(IMediator mediator) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Poll>>> GetPollsAsync()
	{
		var polls = await mediator.Send(new GetPollsQuery());
		return Ok(polls);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<Poll?>> GetPollByIdAsync(int id)
	{
		var poll = await mediator.Send(new GetPollByIdQuery(id));
		if (poll == null)
		{
			return NotFound();
		}

		return Ok(poll);
	}

	[HttpPost]
	public async Task<ActionResult<Poll?>> CreatePollAsync(PollCreateDto poll)
	{
		var newPoll = await mediator.Send(new CreatePollCommand(poll));
		if (newPoll == null)
		{
			return BadRequest();
		}

		return Ok(newPoll);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdatePollAsync(int id, [FromBody] PollUpdateDto pollDto)
	{
		var poll = await mediator.Send(new UpdatePollCommand(id, pollDto));
		if (poll == null)
		{
			return BadRequest();
		}
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult<Poll>> DeletePollAsync(int id)
	{
		await mediator.Send(new DeletePollCommand(id));
		return NoContent();
	}
}
