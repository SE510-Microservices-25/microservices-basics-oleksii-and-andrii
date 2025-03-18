using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Data;
using VoteSystem.Models;

namespace VoteSystem.Query;

public class GetProductsHandler : IRequestHandler<GetVotesQuery, List<Vote>>
{
    private readonly VotesDbContext _context;

    public GetProductsHandler(VotesDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vote>> Handle(GetVotesQuery request, CancellationToken cancellationToken)
    {
        var votes = await _context.Votes.ToListAsync(cancellationToken);

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }
}