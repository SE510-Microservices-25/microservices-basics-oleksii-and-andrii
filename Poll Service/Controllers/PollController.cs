using Microsoft.AspNetCore.Mvc;
using PollSystem.Entities;

namespace PollSystem.Controllers;

[ApiController]
[Route("/[controller]")]
public class PollController : ControllerBase
{
	private static readonly List<Poll> Polls = new();

	[HttpPost]
	public IActionResult CreatePoll([FromBody] PollCreateDto poll)
	{
		Poll newPoll = new Poll(Polls.Count + 1, poll);
		Polls.Add(newPoll);
		return CreatedAtAction(nameof(CreatePoll), new { id = newPoll.Id }, newPoll);
	}

	[HttpGet("{id}")]
	public IActionResult GetPoll(int id)
	{
		var poll = Polls.FirstOrDefault(p => p.Id == id);
		return poll is not null ? Ok(poll) : NotFound();
	}

	[HttpGet]
	public IActionResult GetAllPolls()
	{
		return Ok(Polls);
	}
}
