using MediatR;
using VoteSystem.Command;
using VoteSystem.Models;
using VoteSystem.Services;

namespace VoteSystem.Handlers;

public class CreateVoteHandler(VoteService service) : IRequestHandler<CreateVoteCommand, Vote?>
{
    public async Task<Vote?> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
    {
        return await service.CreateVote(new VoteData(request.PollId, request.UserId, request.ChoiceId),
            cancellationToken);
    }
}

public class DeleteVoteHandler(VoteService service) : IRequestHandler<DeleteVoteCommand, bool>
{
    public async Task<bool> Handle(DeleteVoteCommand request, CancellationToken cancellationToken)
    {
        return await service.DeleteVote(request.Id, cancellationToken);
    }
}