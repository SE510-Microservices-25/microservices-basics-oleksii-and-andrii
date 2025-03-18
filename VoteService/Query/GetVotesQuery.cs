using MediatR;
using VoteSystem.Models;

namespace VoteSystem.Query;

public record GetVotesQuery() : IRequest<List<Vote>>;