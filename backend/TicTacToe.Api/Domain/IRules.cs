namespace TicTacToe.Api.Domain;

/// <summary>
/// Encapsulates Tic Tac Toe rules: move legality, win detection, and draw detection.
/// Current-player validation is the responsibility of Game, not IRules.
/// </summary>
public interface IRules
{
    /// <summary>
    /// Returns true when the position is on the board and the target cell is empty.
    /// Does NOT validate whose turn it is.
    /// </summary>
    bool IsValidMove(Board board, Position position);

    /// <summary>
    /// Returns the winning cells if the given symbol has won; otherwise returns an empty list.
    /// </summary>
    IReadOnlyList<Position> CheckWin(Board board, Symbol symbol);

    /// <summary>
    /// Returns true when the board is full and there is no winner.
    /// Callers must check for a win before checking for a draw.
    /// </summary>
    bool CheckDraw(Board board);
}
