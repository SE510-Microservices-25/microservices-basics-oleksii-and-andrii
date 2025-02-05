using Microsoft.AspNetCore.Mvc;

namespace vote_system;

[ApiController]
[Route("/[controller]")]
public class VoteService : ControllerBase
{
    private readonly Dictionary<int, List<Vote>> _votes = new();

    protected IActionResult RegisterVote(Vote vote)
    {
        if (!_votes.TryGetValue(vote.PollId, out List<Vote>? value))
        {
            value = new List<Vote>();
            _votes[vote.PollId] = value;
        }

        if (value.Contains(vote))
        {
            throw new InvalidOperationException("Vote already registered");
        }

        value.Add(vote);
        return Ok();
    }

    protected IActionResult UnregisterVote(Vote vote)
    {
        if (!_votes.TryGetValue(vote.PollId, out List<Vote>? value))
        {
            throw new InvalidOperationException("Vote not found");
        }

        if (!value.Remove(vote))
        {
            throw new InvalidOperationException("Vote not found");
        }
        return Ok();
    }

    [HttpGet("register/{pollId}/{userId}/{choiceId}")]
    public IActionResult RegisterVote(int pollId, int userId, int choiceId)
    {
        var voteDate = DateTime.Now;
        RegisterVote(new Vote(pollId, userId, choiceId, voteDate));
        return Ok();
    }

    [HttpGet("unregister/{pollId}/{userId}/{choiceId}")]
    public IActionResult UnregisterVote(int pollId, int userId, int choiceId)
    {
        var voteDate = DateTime.Now;
        UnregisterVote(new Vote(pollId, userId, choiceId, voteDate));
        return Ok();
    }

    [HttpGet("{pollId}")]
    public List<Vote> GetVotes(int pollId)
    {
        if (!_votes.TryGetValue(pollId, out List<Vote>? value))
        {
            return new List<Vote>();
        }

        return value;
    }

    [HttpGet("getresult/{pollId}")]
    public Dictionary<int, int> GetResult(int pollId)
    {
        if (!_votes.TryGetValue(pollId, out List<Vote>? value))
        {
            throw new AccessViolationException("Poll not found");
        }

        return value.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(group => group.Key, group => group.Count());
    }
}