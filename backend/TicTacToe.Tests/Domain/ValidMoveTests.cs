using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class ValidMoveTests
{
    [Fact]
    public void Play_ValidMove_PlacesMarkOnBoard()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(0, 0));

        Assert.Equal(Symbol.X, game.Board.GetCell(new Position(0, 0)));
    }

    [Fact]
    public void Play_ValidMove_AddsToMoveHistory()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(1, 1));

        Assert.Single(game.MoveHistory);
        Assert.Equal(1, game.MoveHistory[0].MoveNumber);
        Assert.Equal(Symbol.X, game.MoveHistory[0].Player.Symbol);
        Assert.Equal(new Position(1, 1), game.MoveHistory[0].Position);
    }

    [Fact]
    public void Play_ValidMove_GameRemainsInProgress()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        game.Play(game.PlayerX, new Position(0, 0));

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.Winner);
    }
}
