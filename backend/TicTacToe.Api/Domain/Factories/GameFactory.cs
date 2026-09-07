namespace TicTacToe.Api.Domain;

/// <summary>
/// Constructs fully initialised Game objects according to GameType.
///
/// TwoPlayer: X human, O human, StandardRules, no strategy.
/// Computer:  X human, O computer, StandardRules, BasicComputerMoveStrategy.
/// </summary>
public sealed class GameFactory : IGameFactory
{
    private readonly IRules _rules;
    private readonly IComputerMoveStrategy _computerMoveStrategy;

    public GameFactory(IRules rules, IComputerMoveStrategy computerMoveStrategy)
    {
        _rules = rules;
        _computerMoveStrategy = computerMoveStrategy;
    }

    public Game Create(GameType gameType)
    {
        return gameType switch
        {
            GameType.TwoPlayer => new Game(
                id: Guid.NewGuid(),
                gameType: GameType.TwoPlayer,
                playerX: new Player(Symbol.X, isComputer: false),
                playerO: new Player(Symbol.O, isComputer: false),
                rules: _rules,
                computerMoveStrategy: null),

            GameType.Computer => new Game(
                id: Guid.NewGuid(),
                gameType: GameType.Computer,
                playerX: new Player(Symbol.X, isComputer: false),
                playerO: new Player(Symbol.O, isComputer: true),
                rules: _rules,
                computerMoveStrategy: _computerMoveStrategy),

            _ => throw new ArgumentOutOfRangeException(nameof(gameType), $"Unknown game type: {gameType}")
        };
    }
}
