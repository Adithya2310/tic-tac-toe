namespace TicTacToe.Api.Api.DTOs;

/// <summary>Request body for POST /api/games/{id}/moves.</summary>
public sealed class MakeMoveRequest
{
    /// <summary>"X" or "O".</summary>
    public string Player { get; set; } = string.Empty;

    public int Row { get; set; }
    public int Column { get; set; }
}
