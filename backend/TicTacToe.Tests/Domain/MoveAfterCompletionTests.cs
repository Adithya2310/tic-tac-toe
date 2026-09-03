using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class MoveAfterCompletionTests
{
    [Fact]
    public void Play_AfterWin_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X wins top row
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 0));
        game.Play(game.PlayerX, new Position(0, 1));
        game.Play(game.PlayerO, new Position(1, 1));
        game.Play(game.PlayerX, new Position(0, 2)); // X wins

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerO, new Position(2, 2)));
    }

    [Fact]
    public void Play_AfterDraw_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // Draw sequence
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(0, 2));
        game.Play(game.PlayerO, new Position(1, 2));
        game.Play(game.PlayerX, new Position(1, 0));
        game.Play(game.PlayerO, new Position(2, 0));
        game.Play(game.PlayerX, new Position(1, 1));
        game.Play(game.PlayerO, new Position(2, 2));
        game.Play(game.PlayerX, new Position(2, 1));

        Assert.Equal(GameStatus.Draw, game.Status);
        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerX, new Position(0, 0)));
    }
}
