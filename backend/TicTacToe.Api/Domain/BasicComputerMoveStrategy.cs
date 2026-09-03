namespace TicTacToe.Api.Domain;

/// <summary>
/// Priority-based computer move strategy.
///
/// Evaluation order:
///   1. Take a winning move for O if one exists.
///   2. Block X from winning on the next move.
///   3. Take the center (1,1).
///   4. Take an available corner (deterministic order: TL, TR, BL, BR).
///   5. Take any remaining empty cell.
///
/// The strategy is stateless and deterministic.
/// It clones the board before evaluation so the real game board is never mutated.
/// </summary>
public sealed class BasicComputerMoveStrategy : IComputerMoveStrategy
{
    private static readonly IRules Rules = new StandardRules();

    // Corners in a consistent, deterministic order.
    private static readonly Position[] Corners =
    [
        new(0, 0), new(0, 2), new(2, 0), new(2, 2)
    ];

    public Position GetMove(Board board)
    {
        // 1. O can win — take it.
        var winningMove = FindWinningMove(board, Symbol.O);
        if (winningMove is not null) return winningMove;

        // 2. X can win — block it.
        var blockingMove = FindWinningMove(board, Symbol.X);
        if (blockingMove is not null) return blockingMove;

        // 3. Center.
        var center = new Position(1, 1);
        if (board.IsCellEmpty(center)) return center;

        // 4. Any corner (deterministic order).
        foreach (var corner in Corners)
            if (board.IsCellEmpty(corner)) return corner;

        // 5. Any remaining cell.
        return board.GetEmptyPositions()[0];
    }

    /// <summary>
    /// Checks every empty cell to see if placing <paramref name="symbol"/> there
    /// would immediately win the game. Returns the winning position if found.
    /// </summary>
    private static Position? FindWinningMove(Board board, Symbol symbol)
    {
        foreach (var empty in board.GetEmptyPositions())
        {
            var probe = board.Clone();
            probe.PlaceMark(empty, symbol);
            if (Rules.CheckWin(probe, symbol).Count > 0)
                return empty;
        }
        return null;
    }
}
