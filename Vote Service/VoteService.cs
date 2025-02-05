using Microsoft.AspNetCore.Mvc;

namespace vote_system;

[ApiController]
[Route("/[controller]")]
public class VoteService : ControllerBase
{
    private static Dictionary<int, List<Vote>> _votes = new();

    [HttpGet("register/{pollId}/{userId}/{choiceId}")]
    public IActionResult RegisterVote(int pollId, int userId, int choiceId)
    {
        var voteDate = DateTime.Now;
        var vote = new Vote(pollId, userId, choiceId, voteDate);
        if (!_votes.TryGetValue(vote.PollId, out var value))
        {
            value = new List<Vote>();
            _votes[vote.PollId] = value;
        }

        if (value.Contains(vote))
        {
            return Ok("Vote already registered");
        }

        value.Add(vote);
        return Ok("Vote registered");
    }

    [HttpGet("unregister/{pollId}/{userId}/{choiceId}")]
    public IActionResult UnregisterVote(int pollId, int userId, int choiceId)
    {
        var voteDate = DateTime.Now;
        var vote = new Vote(pollId, userId, choiceId, voteDate);
        if (!_votes.TryGetValue(vote.PollId, out List<Vote>? value))
        {
            return Problem("Poll not found");
        }

        if (!value.Remove(vote))
        {
            return Problem("Vote not found");
        }

        return Ok("Vote unregistered");
    }

    [HttpGet("get_votes/{pollId}")]
    public IActionResult GetVotes(int pollId)
    {
        if (!_votes.TryGetValue(pollId, out List<Vote>? value))
        {
            return Problem("Poll not found");
        }

        return Ok(value);
    }

    [HttpGet("get_result/{pollId}")]
    public IActionResult GetResult(int pollId)
    {
        if (!_votes.TryGetValue(pollId, out List<Vote>? value))
        {
            return Problem("Poll not found");
        }

        var result = value.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(group => group.Key, group => group.Count());

        return Ok(result);
    }
}