using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class ResetGameTests
{
    [Fact]
    public void Reset_ClearsBoard()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));

        game.Reset();

        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(0, 0)));
    }

    [Fact]
    public void Reset_ClearsMoveHistory()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 1));

        game.Reset();

        Assert.Empty(game.MoveHistory);
    }

    [Fact]
    public void Reset_RestoresInitialState()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 0));
        game.Play(game.PlayerX, new Position(0, 1));
        game.Play(game.PlayerO, new Position(1, 1));
        game.Play(game.PlayerX, new Position(0, 2)); // X wins

        game.Reset();

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
        Assert.Equal(Symbol.X, game.CurrentPlayer.Symbol);
    }
}
