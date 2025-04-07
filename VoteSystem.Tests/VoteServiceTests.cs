using MassTransit;
using Moq;
using VoteSystem.Models;
using VoteSystem.Services;

namespace VoteSystem.Tests;

public class VoteServiceTests
{
    private const int PollNum = 5;
    private const int UserNum = 5;
    private const int ChoiceNum = 5;
    private readonly VoteService _voteService;

    public VoteServiceTests()
    {
        var dbDataGen = new DbDataGenerator(PollNum, UserNum, ChoiceNum);
        dbDataGen.Generate();

        var repMock = new RepositoryMock(dbDataGen.GetData());
        repMock.Setup();

        var mockRepository = repMock.Get();
        var mockBus = new Mock<IBus>();
        _voteService = new VoteService(mockRepository.Object, mockBus.Object);
    }

    [Fact]
    public async Task GetAllVotes_ShouldReturnAllVotes()
    {
        var result = await _voteService.GetAllVotes(new CancellationToken(false));

        Assert.NotNull(result);
        Assert.Equal(PollNum * UserNum * ChoiceNum, result.Count);
    }

    [Fact]
    public async Task GetVotes_ShouldReturnVotesForPoll()
    {
        const int pollId = 1;
        var result = await _voteService.GetVotes(pollId, new CancellationToken(false));

        Assert.NotNull(result);
        Assert.Equal(UserNum * ChoiceNum, result.Count);
        Assert.All(result, v => Assert.Equal(pollId, v.PollId));
    }

    [Fact]
    public async Task CreateVote_ShouldReturnCreatedVote()
    {
        var voteData = new VoteData(6, 1, 1);
        var result = await _voteService.CreateVote(voteData, new CancellationToken(false));

        Assert.NotNull(result);
        Assert.Equal(voteData.PollId, result.PollId);
        Assert.Equal(voteData.UserId, result.UserId);
        Assert.Equal(voteData.ChoiceId, result.ChoiceId);
    }

    [Fact]
    public async Task CreateVote_ShouldReturnNull_WhenVoteDataIsNull()
    {
        var result = await _voteService.CreateVote(null, new CancellationToken(false));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteVote_ShouldReturnTrue_WhenVoteExists()
    {
        var result = await _voteService.DeleteVote(1, new CancellationToken(false));

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteVote_ShouldReturnFalse_WhenVoteDoesNotExist()
    {
        var result = await _voteService.DeleteVote(150, new CancellationToken(false));

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllResults_ShouldReturnAllResults()
    {
        var result = await _voteService.GetAllResults(new CancellationToken(false));

        Assert.NotNull(result);
        Assert.Equal(PollNum, result.Count);
        Assert.All(result, r => Assert.Equal(UserNum * ChoiceNum, r.Values.First()));
    }

    [Fact]
    public async Task GetResults_ShouldReturnResultsForPoll()
    {
        const int pollId = 1;
        var result = await _voteService.GetResults(pollId, new CancellationToken(false));

        Assert.NotNull(result);
        Assert.Equal(ChoiceNum, result.Values.Count);
    }

    [Fact]
    public async Task GetResults_ShouldReturnNull_WhenNoVotes()
    {
        const int pollId = PollNum + 1;

        var result = await _voteService.GetResults(pollId, new CancellationToken(false));

        Assert.Null(result);
    }
}