using MediatR;
using PollSystem.Commands;
using PollSystem.Entities;
using PollSystem.Votes;

namespace PollSystem.Commands
{
	public record CreatePollCommand(PollCreateDto Poll) : IRequest<Poll?>;

	public record UpdatePollCommand(int PollId, PollUpdateDto Poll) : IRequest<Poll?>;

	public record DeletePollCommand(int PollId) : IRequest;
}

namespace PollSystem.Handlers
{
	public class CreatePollHandler(PollsService service)
		: IRequestHandler<CreatePollCommand, Poll?>
	{
		public async Task<Poll?> Handle(CreatePollCommand request, CancellationToken cancellationToken)
		{
			return await service.CreatePollAsync(request.Poll, cancellationToken);
		}
	}

	public class UpdatePollHandler(PollsService service)
		: IRequestHandler<UpdatePollCommand, Poll?>
	{
		public async Task<Poll?> Handle(UpdatePollCommand request, CancellationToken cancellationToken)
		{
			return await service.UpdatePollAsync(request.PollId, request.Poll, cancellationToken);
		}
	}

	public class DeletePollHandler(PollsService service)
		: IRequestHandler<DeletePollCommand>
	{
		public async Task Handle(DeletePollCommand request, CancellationToken cancellationToken)
		{
			await service.DeletePollAsync(request.PollId, cancellationToken);
		}
	}
}
