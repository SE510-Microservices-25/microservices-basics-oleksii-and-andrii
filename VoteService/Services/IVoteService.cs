using VoteSystem.Models;

namespace VoteSystem.Services;

public interface IVoteService
{
    Task<List<Vote>> GetAllVotes(CancellationToken cancellationToken);
    Task<List<Vote>> GetVotes(long pollId, CancellationToken cancellationToken);
    Task<Vote?> CreateVote(VoteData? vote, CancellationToken cancellationToken);
    Task<bool> DeleteVote(long id, CancellationToken cancellationToken);
    Task<List<Dictionary<long, int>>?> GetAllResults(CancellationToken cancellationToken);
    Task<Dictionary<long, int>?> GetResults(long pollId, CancellationToken cancellationToken);
}