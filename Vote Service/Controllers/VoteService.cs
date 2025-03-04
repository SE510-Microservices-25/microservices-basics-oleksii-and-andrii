using Microsoft.AspNetCore.Mvc;
using VoteSystem.Models;
using Vote = VoteSystem.Classes.Vote;

namespace VoteSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class VoteService : ControllerBase
{
    private static readonly Dictionary<int, List<Vote>> Votes = new();

    [HttpPost("register/")]
    public IActionResult RegisterVote(VoteData voteData)
    {
        var voteDate = DateTime.Now;
        var vote = new Vote(voteData.PollId, voteData.UserId, voteData.ChoiceId, voteDate);
        if (!Votes.TryGetValue(vote.PollId, out var value))
        {
            value = [];
            Votes[vote.PollId] = value;
        }

        if (value.Contains(vote))
        {
            return Ok("Vote already registered");
        }

        value.Add(vote);
        return Ok(vote);
    }

    [HttpDelete("unregister/")]
    public IActionResult UnregisterVote(VoteData voteData)
    {
        var voteDate = DateTime.Now;
        var vote = new Vote(voteData.PollId, voteData.UserId, voteData.ChoiceId, voteDate);
        if (!Votes.TryGetValue(vote.PollId, out var value))
        {
            return Problem("Poll not found");
        }

        return !value.Remove(vote) ? Problem("Vote not found") : Ok("Vote unregistered");
    }

    [HttpGet("votes/")]
    public IActionResult GetAllVotes([FromRoute] int pollId)
    {
        return Ok(Votes);
    }

    [HttpGet("votes/{pollId:int}")]
    public IActionResult GetVotes([FromRoute] int pollId)
    {
        return !Votes.TryGetValue(pollId, out var value) ? Problem("Poll not found") : Ok(value);
    }

    [HttpGet("result/{pollId:int}")]
    public IActionResult GetResult([FromRoute] int pollId)
    {
        if (!Votes.TryGetValue(pollId, out List<Vote>? value))
        {
            return Problem("Poll not found");
        }

        var result = value.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(group => group.Key, group => group.Count());

        return Ok(result);
    }
}