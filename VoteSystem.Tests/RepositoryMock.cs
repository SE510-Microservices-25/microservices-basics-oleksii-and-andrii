using Moq;
using VoteSystem.Entities;
using VoteSystem.Repository;

namespace VoteSystem.Tests;

public class RepositoryMock(List<VoteEntity> votesList)
{
    private readonly Mock<IVotesRepository> _mockRepository = new();

    public void Setup()
    {
        _mockRepository.Setup(repo => repo.GetAllVotesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(votesList);

        _mockRepository.Setup(repo => repo.GetVoteByPollIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, CancellationToken _) => votesList.Where(v => v.PollId == id).ToList());

        _mockRepository.Setup(repo => repo.CreateVoteAsync(It.IsAny<VoteEntity?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (VoteEntity? vote, CancellationToken _) =>
                {
                    if (vote == null) return null;
                    var newVote = new VoteEntity(vote.PollId, vote.UserId, vote.ChoiceId, DateTime.UtcNow);
                    votesList.Add(newVote);
                    return newVote;
                });

        _mockRepository.Setup(repo => repo.DeleteVoteAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, CancellationToken _) => id < votesList.Count);

        _mockRepository.Setup(repo => repo.GetAllResultsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(votesList.GroupBy(v => v.PollId)
                .Select(g => new Dictionary<long, int> { [g.Key] = g.Count() })
                .ToList());

        _mockRepository.Setup(repo => repo.GetResultsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long pollId, CancellationToken _) =>
            {
                var votes = votesList
                    .Where(v => v.PollId == pollId)
                    .ToList();

                if (votes.Count == 0)
                    return null;

                return votes.GroupBy(vote => vote.ChoiceId)
                    .ToDictionary(g => g.Key, g => g.Count());
            });
    }

    public Mock<IVotesRepository> Get()
    {
        return _mockRepository;
    }
}