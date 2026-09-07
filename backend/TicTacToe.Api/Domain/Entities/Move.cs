namespace TicTacToe.Api.Domain;

/// <summary>
/// An immutable record of a completed move in game history.
/// Move records what happened; Game.Play() performs the move.
/// </summary>
public sealed record Move(int MoveNumber, Player Player, Position Position);
