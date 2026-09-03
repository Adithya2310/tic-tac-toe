namespace TicTacToe.Api.Domain;

/// <summary>
/// Central domain aggregate.
/// Owns the lifecycle of a single Tic Tac Toe game:
///   - Play(player, position): validate and apply a move, check completion.
///   - Undo(): remove the last move(s) and rebuild state.
///   - Reset(): clear board and history, restart from scratch.
///
/// The scoreboard is NOT owned by Game. Scoreboard updates are the service's responsibility.
/// </summary>
public sealed class Game
{
    private readonly IRules _rules;
    private readonly IComputerMoveStrategy? _computerMoveStrategy;
    private readonly List<Move> _moveHistory = [];

    public Guid Id { get; }
    public GameType GameType { get; }
    public Board Board { get; }
    public Player PlayerX { get; }
    public Player PlayerO { get; }
    public Player CurrentPlayer { get; private set; }
    public GameStatus Status { get; private set; }
    public Player? Winner { get; private set; }
    public IReadOnlyList<Position> WinningCells { get; private set; } = [];
    public IReadOnlyList<Move> MoveHistory => _moveHistory;

    public Game(
        Guid id,
        GameType gameType,
        Player playerX,
        Player playerO,
        IRules rules,
        IComputerMoveStrategy? computerMoveStrategy)
    {
        Id = id;
        GameType = gameType;
        PlayerX = playerX;
        PlayerO = playerO;
        CurrentPlayer = playerX; // X always goes first
        Board = new Board();
        Status = GameStatus.InProgress;
        _rules = rules;
        _computerMoveStrategy = computerMoveStrategy;
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
        _moveHistory.Add(move);

        var winningCells = _rules.CheckWin(Board, player.Symbol);
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
    /// TwoPlayer: remove 1 move.
    /// Computer:  remove the latest O (computer) move and the preceding X (human) move.
    ///
    /// The board is rebuilt from the remaining move history to avoid maintaining
    /// a separate mutable undo stack.
    /// </summary>
    public void Undo()
    {
        if (IsCompleted())
            throw new InvalidOperationException("Undo is not available after the game is completed.");

        if (_moveHistory.Count == 0)
            throw new InvalidOperationException("There are no moves to undo.");

        if (GameType == GameType.Computer)
        {
            // In computer mode, the last move in history is always O's (computer) because
            // the service applies the computer move immediately after the human move.
            // If there is only one move in history (X moved, computer has not responded yet),
            // that is an unexpected state — we still allow removal of just that move.
            var lastMove = _moveHistory[^1];

            if (lastMove.Player.Symbol == Symbol.O && _moveHistory.Count >= 2)
            {
                // Normal case: remove O's move and the preceding X move.
                _moveHistory.RemoveAt(_moveHistory.Count - 1);
                _moveHistory.RemoveAt(_moveHistory.Count - 1);
            }
            else
            {
                // Edge case: only one move exists (human moved, computer has not yet).
                _moveHistory.RemoveAt(_moveHistory.Count - 1);
            }
        }
        else
        {
            // Two-player: remove one move.
            _moveHistory.RemoveAt(_moveHistory.Count - 1);
        }

        RebuildBoardFromMoves();
        RecalculateState();
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
    /// Clears the board and replays the remaining move history.
    /// Called after removing move(s) during Undo.
    /// </summary>
    private void RebuildBoardFromMoves()
    {
        Board.Clear();
        foreach (var move in _moveHistory)
            Board.PlaceMark(move.Position, move.Player.Symbol);
    }

    /// <summary>
    /// Recalculates Status, Winner, WinningCells, and CurrentPlayer
    /// based on the current board state after rebuilding from history.
    /// </summary>
    private void RecalculateState()
    {
        // Reset to a clean state before recalculating.
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells = [];

        // Determine whose turn it should be after the remaining moves.
        // X always starts; turns alternate.
        CurrentPlayer = _moveHistory.Count % 2 == 0 ? PlayerX : PlayerO;

        // Check whether the last move (if any) resulted in a win.
        if (_moveHistory.Count > 0)
        {
            var lastPlayer = _moveHistory[^1].Player;
            var winningCells = _rules.CheckWin(Board, lastPlayer.Symbol);
            if (winningCells.Count > 0)
            {
                Status = GameStatus.Won;
                Winner = lastPlayer;
                WinningCells = winningCells;
                return;
            }
        }

        if (_rules.CheckDraw(Board))
            Status = GameStatus.Draw;
    }
}
