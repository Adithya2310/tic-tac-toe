using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

/// <summary>
/// Shared factory helpers for creating domain objects in tests.
/// Avoids repeating construction boilerplate across test classes.
/// </summary>
internal static class GameTestFactory
{
    private static readonly IRules Rules = new StandardRules();

    public static Game CreateTwoPlayerGame() =>
        new(Guid.NewGuid(), GameType.TwoPlayer,
            new Player(Symbol.X, false), new Player(Symbol.O, false),
            Rules);

    public static Game CreateComputerGame() =>
        new(Guid.NewGuid(), GameType.Computer,
            new Player(Symbol.X, false), new Player(Symbol.O, true),
            Rules);
}
