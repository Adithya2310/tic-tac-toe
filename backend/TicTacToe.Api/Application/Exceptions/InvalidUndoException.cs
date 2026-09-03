namespace TicTacToe.Api.Application.Exceptions;

/// <summary>
/// Raised when an undo operation is not available (no moves, game completed,
/// or computer mode pair is incomplete).
/// </summary>
public sealed class InvalidUndoException(string message) : Exception(message);
