namespace TicTacToe.Api.Domain;

/// <summary>
/// Standard 3x3 Tic Tac Toe rules.
/// Checks all rows, columns, and both diagonals for wins.
/// </summary>
public sealed class StandardRules : IRules
{
    public bool IsValidMove(Board board, Position position) =>
        board.IsValidPosition(position) && board.IsCellEmpty(position);

    public IReadOnlyList<Position> CheckWin(Board board, Symbol symbol)
    {
        // Rows
        for (var r = 0; r < 3; r++)
        {
            if (board.GetCell(new Position(r, 0)) == symbol &&
                board.GetCell(new Position(r, 1)) == symbol &&
                board.GetCell(new Position(r, 2)) == symbol)
            {
                return [new Position(r, 0), new Position(r, 1), new Position(r, 2)];
            }
        }

        // Columns
        for (var c = 0; c < 3; c++)
        {
            if (board.GetCell(new Position(0, c)) == symbol &&
                board.GetCell(new Position(1, c)) == symbol &&
                board.GetCell(new Position(2, c)) == symbol)
            {
                return [new Position(0, c), new Position(1, c), new Position(2, c)];
            }
        }

        // Diagonal top-left → bottom-right
        if (board.GetCell(new Position(0, 0)) == symbol &&
            board.GetCell(new Position(1, 1)) == symbol &&
            board.GetCell(new Position(2, 2)) == symbol)
        {
            return [new Position(0, 0), new Position(1, 1), new Position(2, 2)];
        }

        // Diagonal top-right → bottom-left
        if (board.GetCell(new Position(0, 2)) == symbol &&
            board.GetCell(new Position(1, 1)) == symbol &&
            board.GetCell(new Position(2, 0)) == symbol)
        {
            return [new Position(0, 2), new Position(1, 1), new Position(2, 0)];
        }

        return [];
    }

    public bool CheckDraw(Board board) => board.IsFull();
}
