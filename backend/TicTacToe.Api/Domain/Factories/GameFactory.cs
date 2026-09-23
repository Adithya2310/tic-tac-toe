namespace TicTacToe.Api.Domain;

/// <summary>
/// Constructs fully initialised Game objects according to GameType.
///
/// TwoPlayer: X human, O human, StandardRules.
/// Computer:  X human, O computer, StandardRules.
/// Computer move selection is an application-service concern.
/// </summary>
public sealed class GameFactory : IGameFactory
{
    private readonly IRules _rules;

    public GameFactory(IRules rules)
    {
        _rules = rules;
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
                rules: _rules),

            GameType.Computer => new Game(
                id: Guid.NewGuid(),
                gameType: GameType.Computer,
                playerX: new Player(Symbol.X, isComputer: false),
                playerO: new Player(Symbol.O, isComputer: true),
                rules: _rules),

            _ => throw new ArgumentOutOfRangeException(nameof(gameType), $"Unknown game type: {gameType}")
        };
    }
}
