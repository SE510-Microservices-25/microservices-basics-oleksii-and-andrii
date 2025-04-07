using VoteSystem.Entities;

namespace VoteSystem.Tests;

public class DbDataGenerator(int pollNum, int userNum, int choiceNum)
{
    private readonly List<VoteEntity> _votesList = [];

    public void Generate()
    {
        for (var i = 0; i < pollNum; i++)
        {
            for (var j = 0; j < userNum; j++)
            {
                for (var k = 0; k < choiceNum; k++)
                {
                    _votesList.Add(new VoteEntity(i, j, k, DateTime.UtcNow));
                }
            }
        }
    }

    public List<VoteEntity> GetData()
    {
        return _votesList;
    }
}