using VoteSystem.Entities;

namespace VoteSystem.Repository;

public interface IVotesRepository
{
    Task<List<VoteEntity>> GetAllVotesAsync(CancellationToken cancellationToken);
    Task<List<VoteEntity>> GetVoteByPollIdAsync(long id, CancellationToken cancellationToken);
    Task<VoteEntity?> CreateVoteAsync(VoteEntity? vote, CancellationToken cancellationToken);
    Task<bool> DeleteVoteAsync(long id, CancellationToken cancellationToken);
    Task<List<Dictionary<long, int>>?> GetAllResultsAsync(CancellationToken cancellationToken);

    Task<Dictionary<long, int>?> GetResultsAsync(long pollId, CancellationToken cancellationToken);
}