using MediatR;
using PollSystem.Entities;
using PollSystem.Queries;
using PollSystem.Votes;

namespace PollSystem.Queries
{
	public record GetPollsQuery : IRequest<List<Poll>>;

	public record GetPollByIdQuery(int PollId) : IRequest<Poll?>;
}

namespace PollSystem.Handlers
{
	public class GetPollsHandler(PollsService service)
		: IRequestHandler<GetPollsQuery, List<Poll>>, IRequestHandler<GetPollByIdQuery, Poll?>
	{
		public async Task<List<Poll>> Handle(GetPollsQuery request, CancellationToken cancellationToken)
		{
			return await service.GetAllPollsAsync(cancellationToken);
		}

		public async Task<Poll?> Handle(GetPollByIdQuery request, CancellationToken cancellationToken)
		{
			return await service.GetPollByIdAsync(request.PollId, cancellationToken);
		}
	}
}
