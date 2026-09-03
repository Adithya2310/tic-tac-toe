namespace TicTacToe.Api.Api.DTOs;

/// <summary>A single move in the move history list.</summary>
public sealed class MoveResponse
{
    public int MoveNumber { get; set; }
    public string Player { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
}
