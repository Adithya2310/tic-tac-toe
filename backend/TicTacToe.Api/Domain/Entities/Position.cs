namespace TicTacToe.Api.Domain;

/// <summary>
/// A zero-based row/column coordinate on the board.
/// Used for move requests, cell references, and winning-cell sets.
/// </summary>
public sealed record Position(int Row, int Column);
