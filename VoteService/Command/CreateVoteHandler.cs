using MediatR;
using VoteSystem.Data;
using VoteSystem.Entities;
using VoteSystem.Models;

namespace VoteSystem.Command;

public class CreateVoteHandler : IRequestHandler<CreateVoteCommand, Vote>
{
    private readonly VotesDbContext _context;

    public CreateVoteHandler(VotesDbContext context)
    {
        _context = context;
    }

    public async Task<Vote> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
    {
        var dbVote = new VoteEntity(request.PollId, request.UserId, request.ChoiceId, DateTime.UtcNow);

        _context.Votes.Add(dbVote);
        await _context.SaveChangesAsync(cancellationToken);

        return new Vote(dbVote.Id, dbVote.PollId, dbVote.UserId, dbVote.ChoiceId, dbVote.CreatedAt);
    }
}