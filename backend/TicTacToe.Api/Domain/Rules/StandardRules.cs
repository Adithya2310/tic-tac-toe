namespace TicTacToe.Api.Domain;

/// <summary>
/// Standard n×n Tic Tac Toe rules.
///
/// Win detection is O(n) per move: instead of scanning the whole board, only the
/// 4 lines that pass through the last-played cell are checked — the row, the column,
/// the main diagonal (when row == col), and the anti-diagonal (when row + col == n-1).
/// A win is only possible through the cell that was just placed, so nothing else
/// needs to be checked.
///
/// Draw detection is O(n²) in the worst case (IsFull scans all cells) but only
/// runs after a non-winning move, so it is acceptable.
/// </summary>
public sealed class StandardRules : IRules
{
    public bool IsValidMove(Board board, Position position) =>
        board.IsValidPosition(position) && board.IsCellEmpty(position);

    /// <summary>
    /// Checks only the 4 lines through <paramref name="lastPosition"/>.
    /// Returns the full set of winning positions when found; otherwise an empty list.
    /// </summary>
    public IReadOnlyList<Position> CheckWin(Board board, Symbol symbol, Position lastPosition)
    {
        int n   = board.Size;
        int row = lastPosition.Row;
        int col = lastPosition.Column;

        // --- Row ---
        var rowCells = Row(row, n);
        if (AllMatch(board, rowCells, symbol)) return rowCells;

        // --- Column ---
        var colCells = Column(col, n);
        if (AllMatch(board, colCells, symbol)) return colCells;

        // --- Main diagonal (top-left → bottom-right): only when row == col ---
        if (row == col)
        {
            var diagCells = MainDiagonal(n);
            if (AllMatch(board, diagCells, symbol)) return diagCells;
        }

        // --- Anti-diagonal (top-right → bottom-left): only when row + col == n - 1 ---
        if (row + col == n - 1)
        {
            var antiCells = AntiDiagonal(n);
            if (AllMatch(board, antiCells, symbol)) return antiCells;
        }

        return [];
    }

    public bool CheckDraw(Board board) => board.IsFull();

    // ------------------------------------------------------------------
    // Line builders — each returns the n positions that form a line.
    // ------------------------------------------------------------------

    private static IReadOnlyList<Position> Row(int row, int n)
    {
        var cells = new Position[n];
        for (var c = 0; c < n; c++) cells[c] = new Position(row, c);
        return cells;
    }

    private static IReadOnlyList<Position> Column(int col, int n)
    {
        var cells = new Position[n];
        for (var r = 0; r < n; r++) cells[r] = new Position(r, col);
        return cells;
    }

    private static IReadOnlyList<Position> MainDiagonal(int n)
    {
        var cells = new Position[n];
        for (var i = 0; i < n; i++) cells[i] = new Position(i, i);
        return cells;
    }

    private static IReadOnlyList<Position> AntiDiagonal(int n)
    {
        var cells = new Position[n];
        for (var i = 0; i < n; i++) cells[i] = new Position(i, n - 1 - i);
        return cells;
    }

    private static bool AllMatch(Board board, IReadOnlyList<Position> cells, Symbol symbol)
    {
        foreach (var pos in cells)
            if (board.GetCell(pos) != symbol) return false;
        return true;
    }
}
