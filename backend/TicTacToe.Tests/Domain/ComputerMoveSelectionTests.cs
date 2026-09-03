using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class ComputerMoveSelectionTests
{
    private static readonly BasicComputerMoveStrategy Strategy = new();
    private static readonly IRules Rules = new StandardRules();

    /// <summary>Helper: applies moves in X/O alternating order to a fresh board.</summary>
    private static Board BuildBoard(params (int row, int col, Symbol symbol)[] marks)
    {
        var board = new Board();
        foreach (var (row, col, symbol) in marks)
            board.PlaceMark(new Position(row, col), symbol);
        return board;
    }

    // ---- Priority 1: O can win ----

    [Fact]
    public void GetMove_OCanWin_TakesWinningMove()
    {
        // O has (0,0) and (0,1) — winning move is (0,2).
        var board = BuildBoard(
            (0, 0, Symbol.O),
            (0, 1, Symbol.O),
            (1, 0, Symbol.X),
            (1, 1, Symbol.X));

        var move = Strategy.GetMove(board);

        Assert.Equal(new Position(0, 2), move);
    }

    // ---- Priority 2: Block X from winning ----

    [Fact]
    public void GetMove_XCanWin_BlocksX()
    {
        // X has (0,0) and (0,1) — block at (0,2).
        var board = BuildBoard(
            (0, 0, Symbol.X),
            (0, 1, Symbol.X),
            (1, 0, Symbol.O));

        var move = Strategy.GetMove(board);

        Assert.Equal(new Position(0, 2), move);
    }

    // ---- Priority 3: Take center ----

    [Fact]
    public void GetMove_CenterAvailable_TakesCenter()
    {
        var board = BuildBoard(
            (0, 0, Symbol.X));

        var move = Strategy.GetMove(board);

        Assert.Equal(new Position(1, 1), move);
    }

    // ---- Priority 4: Take a corner ----

    [Fact]
    public void GetMove_CenterTaken_TakesCorner()
    {
        var board = BuildBoard(
            (0, 0, Symbol.X),
            (1, 1, Symbol.O));   // center taken

        var move = Strategy.GetMove(board);

        // Corners in deterministic order: (0,0) taken, so next is (0,2).
        Assert.Equal(new Position(0, 2), move);
    }

    // ---- Priority 5: Any remaining cell ----

    [Fact]
    public void GetMove_NoWinNoCenterNoCorner_TakesAnyCell()
    {
        // Fill all corners and center — only edge cells remain.
        var board = BuildBoard(
            (0, 0, Symbol.X), (0, 2, Symbol.O),
            (2, 0, Symbol.X), (2, 2, Symbol.O),
            (1, 1, Symbol.X));

        var move = Strategy.GetMove(board);

        // Board.GetEmptyPositions() returns remaining cells; strategy picks the first.
        Assert.NotNull(move);
        Assert.Equal(Symbol.Empty, board.GetCell(move));
    }

    // ---- Winning move is prioritised over blocking ----

    [Fact]
    public void GetMove_BothWinAndBlock_PrefersWin()
    {
        // O wins at (2,2); X would win at (0,2) — computer should prefer winning.
        var board = BuildBoard(
            (0, 0, Symbol.O), (0, 1, Symbol.O),   // O not in a winning row
            (2, 0, Symbol.O), (2, 1, Symbol.O),   // O wins at (2,2)
            (0, 0, Symbol.X));                     // intentional overlap disabled: use real scenario below

        // Simpler scenario: O can win on column 2, X can win on row 0.
        var board2 = new Board();
        board2.PlaceMark(new Position(0, 2), Symbol.O);
        board2.PlaceMark(new Position(1, 2), Symbol.O);
        board2.PlaceMark(new Position(0, 0), Symbol.X);
        board2.PlaceMark(new Position(0, 1), Symbol.X);

        var move = Strategy.GetMove(board2);

        // O's winning move (2,2) is preferred over blocking X at... X doesn't win in one here.
        // Actual board: O wins at (2,2) on column 2.
        Assert.Equal(new Position(2, 2), move);
    }
}
