namespace TicTacToe.Api.Domain;

/// <summary>
/// Central domain aggregate.
/// Owns the lifecycle of a single Tic Tac Toe game:
///   - Play(player, position): validate and apply a move, check completion.
///   - Undo(): revert the last move(s) in O(1) without rebuilding the board.
///   - Reset(): clear board and history, restart from scratch.
///
/// The scoreboard is NOT owned by Game. Scoreboard updates are the service's responsibility.
///
/// Performance notes
/// -----------------
/// Move history is stored as a Stack&lt;Move&gt; because the only access patterns are
/// push-on-play and pop-on-undo. Using Stack makes the intent explicit and removes
/// the risk of accidental index-based access.
///
/// Undo no longer rebuilds the entire board from history.  Instead it:
///   1. Pops the relevant move(s) from the stack.
///   2. Calls Board.ClearCell() on each popped position — O(1) per move.
///   3. Resets game-status fields to InProgress (undo is only available while InProgress
///      per the assignment policy, so there is no prior-won state to restore).
///   4. Recalculates CurrentPlayer from move count — O(1).
/// </summary>
public sealed class Game
{
    private readonly IRules _rules;

    // Stack gives Push/Pop/Peek semantics that directly express the undo contract.
    // List.RemoveAt(Count-1) was functionally equivalent but masked the intent.
    private readonly Stack<Move> _moveHistory = new();

    public Guid Id { get; }
    public GameType GameType { get; }
    public Board Board { get; }
    public Player PlayerX { get; }
    public Player PlayerO { get; }
    public Player CurrentPlayer { get; private set; }
    public GameStatus Status { get; private set; }
    public Player? Winner { get; private set; }
    public IReadOnlyList<Position> WinningCells { get; private set; } = [];

    /// <summary>
    /// Exposes move history in chronological order (oldest first) for display purposes.
    /// The Stack is reversed on access; callers should not assume O(1) enumeration.
    /// </summary>
    public IReadOnlyList<Move> MoveHistory => [.. _moveHistory.Reverse()];

    public Game(
        Guid id,
        GameType gameType,
        Player playerX,
        Player playerO,
        IRules rules)
    {
        Id = id;
        GameType = gameType;
        PlayerX = playerX;
        PlayerO = playerO;
        CurrentPlayer = playerX; // X always goes first
        Board = new Board();
        Status = GameStatus.InProgress;
        _rules = rules;
    }

    // ------------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------------

    /// <summary>
    /// Applies a move from the given player at the given position.
    ///
    /// Rejects the move when:
    ///   - the game is already complete
    ///   - the wrong player is attempting to move
    ///   - the position is invalid or occupied
    ///
    /// On success: places the mark, records the move, checks for win/draw,
    /// and switches the current player if the game is still in progress.
    /// </summary>
    public void Play(Player player, Position position)
    {
        if (IsCompleted())
            throw new InvalidOperationException("Cannot play a move on a completed game.");

        if (player.Symbol != CurrentPlayer.Symbol)
            throw new InvalidOperationException($"It is {CurrentPlayer.Symbol}'s turn, not {player.Symbol}'s.");

        if (!_rules.IsValidMove(Board, position))
            throw new InvalidOperationException($"Position ({position.Row},{position.Column}) is not a valid move.");

        Board.PlaceMark(position, player.Symbol);

        var move = new Move(_moveHistory.Count + 1, player, position);
        _moveHistory.Push(move);

        // O(n): checks only the 4 lines through the cell that was just placed.
        var winningCells = _rules.CheckWin(Board, player.Symbol, position);
        if (winningCells.Count > 0)
        {
            Status = GameStatus.Won;
            Winner = player;
            WinningCells = winningCells;
            return;
        }

        if (_rules.CheckDraw(Board))
        {
            Status = GameStatus.Draw;
            return;
        }

        SwitchTurn();
    }

    /// <summary>
    /// Undoes the last move (or the last human+computer pair in Computer mode).
    ///
    /// Only permitted while the game is InProgress.
    ///
    /// TwoPlayer: pop 1 move, clear its cell — O(1).
    /// Computer:  pop O's move and X's preceding move, clear both cells — O(1).
    ///
    /// Board state is updated incrementally via ClearCell; the board is NOT rebuilt
    /// from scratch, avoiding the O(n²) replay overhead.
    /// </summary>
    public void Undo()
    {
        if (IsCompleted())
            throw new InvalidOperationException("Undo is not available after the game is completed.");

        if (_moveHistory.Count == 0)
            throw new InvalidOperationException("There are no moves to undo.");

        // Every undo removes the latest move and returns the turn to that player.
        UndoSingleMove();
        SwitchTurn();

        if (GameType == GameType.Computer)
        {
            // Computer mode also removes the preceding human move, returning to X.
            UndoSingleMove();
            SwitchTurn();
        }
    }

    /// <summary>
    /// Resets the game to its initial state.
    /// Board, history, status, winner, and winning cells are cleared.
    /// Current player returns to X.
    /// The scoreboard is NOT modified.
    /// </summary>
    public void Reset()
    {
        Board.Clear();
        _moveHistory.Clear();
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];
        CurrentPlayer = PlayerX;
    }

    // ------------------------------------------------------------------
    // Internal helpers
    // ------------------------------------------------------------------

    private bool IsCompleted() =>
        Status == GameStatus.Won || Status == GameStatus.Draw;

    private void SwitchTurn() =>
        CurrentPlayer = CurrentPlayer.Symbol == Symbol.X ? PlayerO : PlayerX;

    /// <summary>
    /// Pops the top move from the stack and clears its cell on the board.
    /// This is O(1) — no board replay required.
    /// </summary>
    private void UndoSingleMove()
    {
        var move = _moveHistory.Pop();
        Board.ClearCell(move.Position);
    }
}
