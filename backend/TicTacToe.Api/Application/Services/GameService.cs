using TicTacToe.Api.Application.Exceptions;
using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Application;

/// <summary>
/// Orchestrates game use cases: CreateGame, GetGame, MakeMove, Undo, ResetGame.
///
/// Scoreboard updates after game completion are the responsibility of this service,
/// not of the Game domain object.
///
/// In Computer mode, MakeMove applies the human move and then immediately invokes
/// IComputerMoveStrategy to select and apply the computer's response — all within
/// the same HTTP request.
/// </summary>
public sealed class GameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IScoreboardRepository _scoreboardRepository;
    private readonly IGameFactory _gameFactory;
    private readonly IComputerMoveStrategy _computerMoveStrategy;

    public GameService(
        IGameRepository gameRepository,
        IScoreboardRepository scoreboardRepository,
        IGameFactory gameFactory,
        IComputerMoveStrategy computerMoveStrategy)
    {
        _gameRepository = gameRepository;
        _scoreboardRepository = scoreboardRepository;
        _gameFactory = gameFactory;
        _computerMoveStrategy = computerMoveStrategy;
    }

    /// <summary>Creates and persists a new game of the given type.</summary>
    public Game CreateGame(GameType gameType)
    {
        var game = _gameFactory.Create(gameType);
        _gameRepository.Add(game);
        return game;
    }

    /// <summary>Returns the current authoritative game state.</summary>
    public Game GetGame(Guid gameId)
    {
        return _gameRepository.GetById(gameId)
            ?? throw new GameNotFoundException(gameId);
    }

    /// <summary>
    /// Applies a human move. For Computer mode, also applies the computer's
    /// response immediately if the game is still in progress after the human move.
    ///
    /// Updates the scoreboard exactly once if the move (human or computer) completes the game.
    /// </summary>
    public Game MakeMove(Guid gameId, Symbol playerSymbol, Position position)
    {
        var game = GetGame(gameId);
        var player = ResolvePlayer(game, playerSymbol);

        try
        {
            game.Play(player, position);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidMoveException(ex.Message);
        }

        // If the human move ended the game, update scoreboard and return.
        if (game.Status != GameStatus.InProgress)
        {
            UpdateScoreboard(game);
            _gameRepository.Update(game);
            return game;
        }

        // Computer mode: select and apply the computer's move immediately.
        if (game.GameType == GameType.Computer)
        {
            var computerPosition = _computerMoveStrategy.GetMove(game.Board);
            game.Play(game.PlayerO, computerPosition);

            if (game.Status != GameStatus.InProgress)
                UpdateScoreboard(game);
        }

        _gameRepository.Update(game);
        return game;
    }

    /// <summary>
    /// Undoes the last move (or human+computer pair in Computer mode).
    /// Only available while the game is InProgress.
    /// The scoreboard is not rolled back.
    /// </summary>
    public Game Undo(Guid gameId)
    {
        var game = GetGame(gameId);

        try
        {
            game.Undo();
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidUndoException(ex.Message);
        }

        _gameRepository.Update(game);
        return game;
    }

    /// <summary>
    /// Resets the game board and history without affecting the scoreboard.
    /// </summary>
    public Game ResetGame(Guid gameId)
    {
        var game = GetGame(gameId);
        game.Reset();
        _gameRepository.Update(game);
        return game;
    }

    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------

    private static Player ResolvePlayer(Game game, Symbol symbol)
    {
        if (symbol == game.PlayerX.Symbol) return game.PlayerX;
        if (symbol == game.PlayerO.Symbol) return game.PlayerO;
        throw new InvalidMoveException($"Symbol '{symbol}' does not belong to either player in this game.");
    }

    private void UpdateScoreboard(Game game)
    {
        var scoreboard = _scoreboardRepository.Get();

        if (game.Status == GameStatus.Won && game.Winner is not null)
            scoreboard.RecordWin(game.Winner.Symbol);
        else if (game.Status == GameStatus.Draw)
            scoreboard.RecordDraw();

        _scoreboardRepository.Save(scoreboard);
    }
}
