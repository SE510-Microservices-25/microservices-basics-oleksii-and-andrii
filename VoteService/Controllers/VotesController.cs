using Microsoft.AspNetCore.Mvc;
using VoteSystem.Models;
using VoteSystem.Services;

namespace VoteSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class VotesController(VoteService voteService) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult GetAllVotes()
    {
        return Ok(voteService.GetAllVotes());
    }

    [HttpGet("/{pollId:int}")]
    public IActionResult GetVotes([FromRoute] int pollId)
    {
        var votes = voteService.GetVotes(pollId);
        return votes is null ? Problem("Poll not found") : Ok(votes);
    }

    [HttpPost("/")]
    public IActionResult RegisterVote(VoteData voteData)
    {
        var vote = voteService.CreateVote(voteData).Result;
        return vote == null ? Problem("Vote not registered") : Ok(vote);
    }

    [HttpDelete("/")]
    public IActionResult UnregisterVote(long id)
    {
        var voteDeleted = voteService.DeleteVote(id).Result;

        return voteDeleted ? NoContent() : NotFound("Vote not found");
    }

    [HttpGet("/result/")]
    public IActionResult GetAllResults()
    {
        var result = voteService.GetAllVotesByPoll();
        return result == null ? Problem("No votes registered") : Ok(result);
    }

    [HttpGet("/result/{pollId:int}")]
    public IActionResult GetResult([FromRoute] int pollId)
    {
        var result = voteService.GetVotesByPoll(pollId);
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