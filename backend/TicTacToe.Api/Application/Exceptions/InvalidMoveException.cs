namespace TicTacToe.Api.Application.Exceptions;

/// <summary>
/// Raised when a move is rejected by the domain (wrong player, occupied cell,
/// out-of-bounds position, or game already completed).
/// </summary>
public sealed class InvalidMoveException(string message) : Exception(message);
