namespace TicTacToe.Api.Api.DTOs;

/// <summary>Request body for POST /api/games.</summary>
public sealed class CreateGameRequest
{
    /// <summary>"TwoPlayer" or "Computer".</summary>
    public string GameType { get; set; } = string.Empty;
}
