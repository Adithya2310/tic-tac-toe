using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class InvalidMoveTests
{
    [Fact]
    public void Play_OccupiedCell_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));

        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerO, new Position(0, 0)));
    }

    [Fact]
    public void Play_OutOfBoundsPosition_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerX, new Position(3, 0)));
    }

    [Fact]
    public void Play_NegativePosition_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerX, new Position(-1, 0)));
    }
}
