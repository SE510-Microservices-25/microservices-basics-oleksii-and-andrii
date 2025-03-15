namespace VoteSystem.Contracts;

public class VoteCreated(int id, int pollId, int userId, int choiceId, DateTime date)
{
    public int Id { get; set; } = id;
    public int PollId { get; set; } = pollId;
    public int UserId { get; set; } = userId;
    public int ChoiceId { get; set; } = choiceId;
    public DateTime Date { get; set; } = date;
}