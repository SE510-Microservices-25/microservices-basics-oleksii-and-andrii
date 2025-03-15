namespace VoteSystem.Models;

public record Vote(long Id, long PollId, long UserId, long ChoiceId, DateTime CreatedAt);

public record VoteData(int PollId, int UserId, int ChoiceId);