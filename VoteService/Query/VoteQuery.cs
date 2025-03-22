using MediatR;
using VoteSystem.Models;

namespace VoteSystem.Query;

public record GetVotesQuery : IRequest<List<Vote>>;
public record GetVotesByPollQuery(int PollId) : IRequest<List<Vote>>;
public record GetAllResultsQuery : IRequest<List<Dictionary<long, int>>?>;
public record GetResultsQuery(int PollId) : IRequest<Dictionary<long, int>?>;