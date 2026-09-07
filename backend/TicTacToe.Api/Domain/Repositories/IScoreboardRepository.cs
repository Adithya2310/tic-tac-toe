namespace TicTacToe.Api.Domain;

/// <summary>
/// Persistence contract for the session Scoreboard.
/// There is exactly one scoreboard per application session.
/// </summary>
public interface IScoreboardRepository
{
    Scoreboard Get();
    void Save(Scoreboard scoreboard);
}
