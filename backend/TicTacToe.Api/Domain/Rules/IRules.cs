namespace TicTacToe.Api.Domain;

/// <summary>
/// Encapsulates Tic Tac Toe rules: move legality, win detection, and draw detection.
/// Current-player validation is the responsibility of Game, not IRules.
/// </summary>
public interface IRules
{
    /// <summary>
    /// Returns true when the position is within the board and the target cell is empty.
    /// Does NOT validate whose turn it is.
    /// </summary>
    bool IsValidMove(Board board, Position position);

    /// <summary>
    /// Returns the winning cells if the given symbol has won after placing at lastPosition;
    /// otherwise returns an empty list.
    /// Receives lastPosition so implementations can check only the 4 lines through that cell
    /// instead of scanning the entire board.
    /// </summary>
    IReadOnlyList<Position> CheckWin(Board board, Symbol symbol, Position lastPosition);

    /// <summary>
    /// Returns true when the board is full and there is no winner.
    /// Callers must check for a win before checking for a draw.
    /// </summary>
    bool CheckDraw(Board board);
}
