namespace TicTacToe.Api.Api.DTOs;

/// <summary>Request body for POST /api/games.</summary>
public sealed class CreateGameRequest
{
    /// <summary>"TwoPlayer" or "Computer".</summary>
    public string GameType { get; set; } = string.Empty;

    /// <summary>
    /// The side length of the board (n for an n×n grid). Must be at least 3.
    /// Defaults to 3 when the client omits this field, preserving backward compatibility.
    /// </summary>
    public int BoardSize { get; set; } = 3;
}
