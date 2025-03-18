using MediatR;
using VoteSystem.Models;

namespace VoteSystem.Command;

public record CreateVoteCommand(long PollId, long UserId, long ChoiceId) : IRequest<Vote>;