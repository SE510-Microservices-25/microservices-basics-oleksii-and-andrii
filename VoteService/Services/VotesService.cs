using VoteSystem.Entities;
using VoteSystem.Models;
using VoteSystem.Repository;

namespace VoteSystem.Services;

public class VoteService(VotesRepository repository)
{
    public async Task<List<Vote>> GetAllVotes(CancellationToken cancellationToken = default)
    {
        var votes = await repository.GetAllVotesAsync(cancellationToken);

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public async Task<List<Vote>> GetVotes(long pollId, CancellationToken cancellationToken = default)
    {
        var votes = await repository.GetVoteByPollIdAsync(pollId, cancellationToken);

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public async Task<Vote?> CreateVote(VoteData? vote, CancellationToken cancellationToken = default)
    {
        if (vote == null) return null;

        var voteEntity = new VoteEntity(vote.PollId, vote.UserId, vote.ChoiceId, DateTime.UtcNow);

        var dbVote = await repository.CreateVoteAsync(voteEntity, cancellationToken);

        return dbVote == null
            ? null
            : new Vote(dbVote.Id, dbVote.PollId, dbVote.UserId, dbVote.ChoiceId, dbVote.CreatedAt);
    }

    public async Task<bool> DeleteVote(long id, CancellationToken cancellationToken = default)
    {
        var result = await repository.DeleteVoteAsync(id, cancellationToken);
        return result;
    }

    public async Task<List<Dictionary<long, int>>?> GetAllResults(CancellationToken cancellationToken = default)
    {
        var results = await repository.GetAllResultsAsync(cancellationToken);

        return results;
    }

    public async Task<Dictionary<long, int>?> GetResults(long pollId, CancellationToken cancellationToken = default)
    {
        var results = await repository.GetResultsAsync(pollId, cancellationToken);
        return results;
    }
}