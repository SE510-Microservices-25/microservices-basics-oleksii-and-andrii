namespace VoteSystem.Models;

public class Vote
{
    public int Id { get; set; }
    public int PollId { get; set; }
    public int UserId { get; set; }
    public int ChoiceId { get; set; }
    public DateTime Date { get; set; }
}