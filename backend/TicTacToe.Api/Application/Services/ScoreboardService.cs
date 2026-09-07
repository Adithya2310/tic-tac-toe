using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Application;

/// <summary>
/// Orchestrates scoreboard use cases: GetScoreboard, ResetScoreboard.
/// </summary>
public sealed class ScoreboardService
{
    private readonly IScoreboardRepository _scoreboardRepository;

    public ScoreboardService(IScoreboardRepository scoreboardRepository)
    {
        _scoreboardRepository = scoreboardRepository;
    }

    public Scoreboard GetScoreboard() => _scoreboardRepository.Get();

    public Scoreboard ResetScoreboard()
    {
        var scoreboard = _scoreboardRepository.Get();
        scoreboard.Reset();
        _scoreboardRepository.Save(scoreboard);
        return scoreboard;
    }
}
