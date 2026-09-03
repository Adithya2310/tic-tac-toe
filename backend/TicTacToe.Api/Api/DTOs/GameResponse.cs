namespace TicTacToe.Api.Api.DTOs;

/// <summary>
/// The authoritative game state returned by all game endpoints.
/// The frontend must render this response directly rather than recomputing state.
/// </summary>
public sealed class GameResponse
{
    public Guid GameId { get; set; }

    /// <summary>3x3 board: board[row][col] is "X", "O", or "".</summary>
    public List<List<string>> Board { get; set; } = [];

    /// <summary>"X" or "O".</summary>
    public string CurrentPlayer { get; set; } = string.Empty;

    /// <summary>"TwoPlayer" or "Computer".</summary>
    public string GameType { get; set; } = string.Empty;

    /// <summary>"InProgress", "Won", or "Draw".</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Null until the game is won.</summary>
    public string? Winner { get; set; }

    public List<WinningCellResponse> WinningCells { get; set; } = [];

    public List<MoveResponse> Moves { get; set; } = [];
}

/// <summary>A single winning cell coordinate.</summary>
public sealed class WinningCellResponse
{
    public int Row { get; set; }
    public int Column { get; set; }
}
