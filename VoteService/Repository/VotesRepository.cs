using Microsoft.EntityFrameworkCore;
using VoteSystem.Data;
using VoteSystem.Entities;

namespace VoteSystem.Repository;

public class VotesRepository(VotesDbContext context) : IVotesRepository
{
    public async Task<List<VoteEntity>> GetAllVotesAsync(CancellationToken cancellationToken)
    {
        return await context.Votes.ToListAsync(cancellationToken);
    }

    public async Task<List<VoteEntity>> GetVoteByPollIdAsync(long id, CancellationToken cancellationToken)
    {
        return await context.Votes.Where(v => v.PollId == id).ToListAsync(cancellationToken);
    }

    public async Task<VoteEntity?> CreateVoteAsync(VoteEntity? vote, CancellationToken cancellationToken)
    {
        if (vote == null) return null;

        try
        {
            await context.Votes.AddAsync(vote, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

        return vote;
    }

    public async Task<bool> DeleteVoteAsync(long id, CancellationToken cancellationToken)
    {
        var vote = await context.Votes.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vote == null) return false;

        try
        {
            context.Votes.Remove(vote);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

        return true;
    }

    public async Task<List<Dictionary<long, int>>?> GetAllResultsAsync(CancellationToken cancellationToken)
    {
        var votesGrouped = await context.Votes
            .GroupBy(v => v.PollId)
            .ToListAsync(cancellationToken);

        if (votesGrouped.Count == 0)
            return null;

        return votesGrouped.Select(group =>
                group.GroupBy(vote => vote.ChoiceId)
                    .ToDictionary(g => g.Key, g => g.Count()))
            .ToList();
    }

    public async Task<Dictionary<long, int>?> GetResultsAsync(long pollId, CancellationToken cancellationToken)
    {
        var votes = await context.Votes
            .Where(v => v.PollId == pollId)
            .ToListAsync(cancellationToken);

        if (votes.Count == 0)
            return null;

        return votes.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}