using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class DrawTests
{
    [Fact]
    public void Play_BoardFullWithNoWinner_StatusIsDraw()
    {
        // Board after all 9 moves (no winner):
        // X O X
        // X X O
        // O X O
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(0, 0)); // X
        game.Play(game.PlayerO, new Position(0, 1)); // O
        game.Play(game.PlayerX, new Position(0, 2)); // X
        game.Play(game.PlayerO, new Position(1, 2)); // O
        game.Play(game.PlayerX, new Position(1, 0)); // X
        game.Play(game.PlayerO, new Position(2, 0)); // O
        game.Play(game.PlayerX, new Position(1, 1)); // X
        game.Play(game.PlayerO, new Position(2, 2)); // O
        game.Play(game.PlayerX, new Position(2, 1)); // X

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
    }
}
