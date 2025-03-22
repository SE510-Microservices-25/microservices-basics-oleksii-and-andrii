using MediatR;
using VoteSystem.Models;
using VoteSystem.Query;
using VoteSystem.Services;

namespace VoteSystem.Handlers;

public class GetVotesHandler(VoteService service)
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

public class GerResultsHandler(VoteService service)
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