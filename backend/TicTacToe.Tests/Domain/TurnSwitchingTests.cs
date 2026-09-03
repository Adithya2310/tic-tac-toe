using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class TurnSwitchingTests
{
    [Fact]
    public void Play_AfterXMoves_CurrentPlayerIsO()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(0, 0));

        Assert.Equal(Symbol.O, game.CurrentPlayer.Symbol);
    }

    [Fact]
    public void Play_AfterXThenO_CurrentPlayerIsX()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 1));

        Assert.Equal(Symbol.X, game.CurrentPlayer.Symbol);
    }

    [Fact]
    public void Play_WrongPlayer_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        // X's turn; O tries to move.
        Assert.Throws<InvalidOperationException>(() =>
            game.Play(game.PlayerO, new Position(0, 0)));
    }
}
