namespace VoteSystem.Classes;

public class Vote(int pollId, int userId, int choiceId, DateTime date)
{
    public int PollId { get; } = pollId;
    public int UserId { get; } = userId;
    public int ChoiceId { get; } = choiceId;
    public DateTime Date { get; } = date;

    public override bool Equals(object? obj)
    {
        if (obj is Vote vote)
        {
            return PollId == vote.PollId && UserId == vote.UserId && ChoiceId == vote.ChoiceId;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PollId, UserId, ChoiceId);
    }
}