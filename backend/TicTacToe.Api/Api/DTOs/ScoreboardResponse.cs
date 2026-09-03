namespace TicTacToe.Api.Api.DTOs;

/// <summary>Response body for GET /api/scoreboard.</summary>
public sealed class ScoreboardResponse
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
}
