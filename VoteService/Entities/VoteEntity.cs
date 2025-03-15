namespace VoteSystem.Entities;

public class VoteEntity(long pollId, long userId, long choiceId, DateTime createdAt)
{
    public long Id { get; set; }
    public long PollId { get; set; } = pollId;
    public long UserId { get; set; } = userId;
    public long ChoiceId { get; set; } = choiceId;
    public DateTime CreatedAt { get; set; } = createdAt;
}