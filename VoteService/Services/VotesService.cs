using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Data;
using VoteSystem.Entities;
using VoteSystem.Models;

namespace VoteSystem.Services;

public class VoteService(VotesDbContext context)
{
    public List<Vote> GetAllVotes()
    {
        var votes = context.Votes.ToList();

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public List<Vote>? GetVotes(long pollId)
    {
        var votes = context.Votes.Where(v => v.PollId == pollId).ToList();

        if (votes.Count == 0) return null;

        return votes.Select(v => new Vote(v.Id, v.PollId, v.UserId, v.ChoiceId, v.CreatedAt)).ToList();
    }

    public async Task<Vote?> CreateVote(VoteData? vote)
    {
        if (vote == null) return null;

        var dbVote = new VoteEntity(vote.PollId, vote.UserId, vote.ChoiceId, DateTime.UtcNow);

        try
        {
            context.Votes.Add(dbVote);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

        return new Vote(dbVote.Id, dbVote.PollId, dbVote.UserId, dbVote.ChoiceId, dbVote.CreatedAt);
    }

    public async Task<bool> DeleteVote(long id)
    {
        var vote = context.Votes.FirstOrDefault(v => v.Id == id);

        if (vote == null) return false;

        try
        {
            context.Votes.Remove(vote);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public List<Dictionary<long, int>>? GetAllVotesByPoll()
    {
        var votes = context.Votes.GroupBy(v => v.PollId).ToList();

        if (votes.Count == 0) return null;

        return votes.Select(v => v.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(group => group.Key, group => group.Count())).ToList();
    }

    public Dictionary<long, int>? GetVotesByPoll(long pollId)
    {
        var votes = context.Votes.Where(v => v.PollId == pollId).ToList();

        if (votes.Count == 0) return null;

        return votes.GroupBy(vote => vote.ChoiceId)
            .ToDictionary(group => group.Key, group => group.Count());
    }
}