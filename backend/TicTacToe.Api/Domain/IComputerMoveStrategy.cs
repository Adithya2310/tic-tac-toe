namespace TicTacToe.Api.Domain;

/// <summary>
/// Selects a position for the computer player given the current board state.
/// Implementations must not mutate the real board.
/// </summary>
public interface IComputerMoveStrategy
{
    /// <summary>
    /// Returns the position the computer should play on the given board.
    /// The board is guaranteed to have at least one empty cell.
    /// </summary>
    Position GetMove(Board board);
}
