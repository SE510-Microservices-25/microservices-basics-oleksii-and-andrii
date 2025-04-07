using MediatR;
using VoteSystem.Models;
using VoteSystem.Query;
using VoteSystem.Services;

namespace VoteSystem.Query
{
    public record GetVotesQuery : IRequest<List<Vote>>;

    public record GetVotesByPollQuery(int PollId) : IRequest<List<Vote>>;

    public record GetAllResultsQuery : IRequest<List<Dictionary<long, int>>?>;

    public record GetResultsQuery(int PollId) : IRequest<Dictionary<long, int>?>;
}

namespace VoteSystem.Handlers
{
    public class GetVotesHandler(IVoteService service)
        : IRequestHandler<GetVotesQuery, List<Vote>>, IRequestHandler<GetVotesByPollQuery, List<Vote>>
    {
        public async Task<List<Vote>> Handle(GetVotesQuery request, CancellationToken cancellationToken)
        {
            return await service.GetAllVotes(cancellationToken);
        }

        public async Task<List<Vote>> Handle(GetVotesByPollQuery request, CancellationToken cancellationToken)
        {
            return await service.GetVotes(request.PollId, cancellationToken);
        }
    }

    public class GetResultsHandler(IVoteService service)
        : IRequestHandler<GetAllResultsQuery, List<Dictionary<long, int>>?>,
            IRequestHandler<GetResultsQuery, Dictionary<long, int>?>
    {
        public async Task<List<Dictionary<long, int>>?> Handle(GetAllResultsQuery request,
            CancellationToken cancellationToken)
        {
            return await service.GetAllResults(cancellationToken);
        }

        public async Task<Dictionary<long, int>?> Handle(GetResultsQuery request, CancellationToken cancellationToken)
        {
            return await service.GetResults(request.PollId, cancellationToken);
        }
    }
}