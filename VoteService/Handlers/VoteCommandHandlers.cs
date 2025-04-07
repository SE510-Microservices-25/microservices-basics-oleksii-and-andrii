using MediatR;
using VoteSystem.Command;
using VoteSystem.Models;
using VoteSystem.Services;

namespace VoteSystem.Command
{
    public record CreateVoteCommand(long PollId, long UserId, long ChoiceId) : IRequest<Vote?>;

    public record DeleteVoteCommand(long Id) : IRequest<bool>;
}

namespace VoteSystem.Handlers
{
    public class CreateVoteHandler(IVoteService service) : IRequestHandler<CreateVoteCommand, Vote?>
    {
        public async Task<Vote?> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
        {
            return await service.CreateVote(new VoteData(request.PollId, request.UserId, request.ChoiceId),
                cancellationToken);
        }
    }

    public class DeleteVoteHandler(IVoteService service) : IRequestHandler<DeleteVoteCommand, bool>
    {
        public async Task<bool> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
        {
            return await service.DeleteVote(request.Id, cancellationToken);
        }
    }
}