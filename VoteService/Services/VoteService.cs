using MassTransit;
using VoteSystem.Entities;
using VoteSystem.Models;
using VoteSystem.Repository;

namespace VoteSystem.Services;

public class VoteService(IVotesRepository repository, IBus bus) : IVoteService
{
    public async Task<List<Vote>> GetAllVotes(CancellationToken cancellationToken)
    {
        var votes = await repository.GetAllVotesAsync(cancellationToken);

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public async Task<List<Vote>> GetVotes(long pollId, CancellationToken cancellationToken)
    {
        var votes = await repository.GetVoteByPollIdAsync(pollId, cancellationToken);

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public async Task<Vote?> CreateVote(VoteData? vote, CancellationToken cancellationToken)
    {
        if (vote == null) return null;

        var voteEntity = new VoteEntity(vote.PollId, vote.UserId, vote.ChoiceId, DateTime.UtcNow);

        var dbVote = await repository.CreateVoteAsync(voteEntity, cancellationToken);
        if (dbVote == null) return null;

        var newVote = new Vote(dbVote.Id, dbVote.PollId, dbVote.UserId, dbVote.ChoiceId, dbVote.CreatedAt);
        await bus.Publish(newVote, cancellationToken);
        return newVote;
    }

    public async Task<bool> DeleteVote(long id, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteVoteAsync(id, cancellationToken);
        return result;
    }

    public async Task<List<Dictionary<long, int>>?> GetAllResults(CancellationToken cancellationToken)
    {
        var results = await repository.GetAllResultsAsync(cancellationToken);
        return results;
    }

    public async Task<Dictionary<long, int>?> GetResults(long pollId, CancellationToken cancellationToken)
    {
        var results = await repository.GetResultsAsync(pollId, cancellationToken);
        return results;
    }
}