namespace vote_system;

public class Vote(int pollId, int userId, int choiceId, DateTime voteDate)
{
    public int PollId { get; private set; } = pollId;
    private int UserId { get; set; } = userId;
    public int ChoiceId { get; private set; } = choiceId;
    private DateTime VoteDate { get; set; } = voteDate;

    public bool Equals(Vote? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PollId == other.PollId && UserId == other.UserId &&
               ChoiceId == other.ChoiceId && VoteDate.Equals(other.VoteDate);
    }
}