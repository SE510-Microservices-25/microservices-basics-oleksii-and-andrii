using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoteSystem.Command;
using VoteSystem.Models;
using VoteSystem.Query;

namespace VoteSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class VotesController(IMediator mediator) : ControllerBase
{
    [HttpGet("/")]
    public async Task<IActionResult> GetAllVotes()
    {
        var votes = await mediator.Send(new GetVotesQuery());
        return Ok(votes);
    }

    [HttpGet("/{pollId:int}")]
    public async Task<IActionResult> GetVotes([FromRoute] int pollId)
    {
        var votes = await mediator.Send(new GetVotesByPollQuery(pollId));
        return votes is null ? Problem("Poll not found") : Ok(votes);
    }

    [HttpPost("/")]
    public async Task<IActionResult> RegisterVote(VoteData vd)
    {
        var vote = await mediator.Send(new CreateVoteCommand(vd.PollId, vd.UserId, vd.ChoiceId));
        return vote == null ? Problem("Vote not registered") : Ok(vote);
    }

    [HttpDelete("/")]
    public async Task<IActionResult> UnregisterVote(long id)
    {
        var voteDeleted = await mediator.Send(new DeleteVoteCommand(id));

        return voteDeleted ? NoContent() : NotFound("Vote not found");
    }

    [HttpGet("/result/")]
    public async Task<IActionResult> GetAllResults()
    {
        var result = await mediator.Send(new GetAllResultsQuery());
        return result == null ? Problem("No votes registered") : Ok(result);
    }

    [HttpGet("/result/{pollId:int}")]
    public async Task<IActionResult> GetResult([FromRoute] int pollId)
    {
        var result = await mediator.Send(new GetResultsQuery(pollId));
        return result == null ? Problem("Poll not found") : Ok(result);
    }

    [HttpGet("get-polls")]
    public async Task<IActionResult> GetPolls()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://poll-service:5002/Polls/");
        var polls = await response.Content.ReadAsStringAsync();
        return Ok(polls);
    }
}