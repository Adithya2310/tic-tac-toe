using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class WinDetectionTests
{
    // ---- Row wins ----

    [Fact]
    public void Play_TopRowWin_StatusIsWon()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (0,0) (0,1) (0,2) / O: (1,0) (1,1)
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 0));
        game.Play(game.PlayerX, new Position(0, 1));
        game.Play(game.PlayerO, new Position(1, 1));
        game.Play(game.PlayerX, new Position(0, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Symbol.X, game.Winner?.Symbol);
        Assert.Equal(3, game.WinningCells.Count);
    }

    [Fact]
    public void Play_MiddleRowWin_WinningCellsCorrect()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (1,0) (1,1) (1,2) / O: (0,0) (0,1)
        game.Play(game.PlayerX, new Position(1, 0));
        game.Play(game.PlayerO, new Position(0, 0));
        game.Play(game.PlayerX, new Position(1, 1));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(1, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Contains(new Position(1, 0), game.WinningCells);
        Assert.Contains(new Position(1, 1), game.WinningCells);
        Assert.Contains(new Position(1, 2), game.WinningCells);
    }

    // ---- Column wins ----

    [Fact]
    public void Play_LeftColumnWin_StatusIsWon()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (0,0) (1,0) (2,0) / O: (0,1) (0,2)
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(1, 0));
        game.Play(game.PlayerO, new Position(0, 2));
        game.Play(game.PlayerX, new Position(2, 0));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Equal(Symbol.X, game.Winner?.Symbol);
    }

    [Fact]
    public void Play_RightColumnWin_WinningCellsCorrect()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (0,2) (1,2) (2,2) / O: (0,0) (0,1)
        game.Play(game.PlayerX, new Position(0, 2));
        game.Play(game.PlayerO, new Position(0, 0));
        game.Play(game.PlayerX, new Position(1, 2));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(2, 2));

        Assert.Contains(new Position(0, 2), game.WinningCells);
        Assert.Contains(new Position(1, 2), game.WinningCells);
        Assert.Contains(new Position(2, 2), game.WinningCells);
    }

    // ---- Diagonal wins ----

    [Fact]
    public void Play_MainDiagonalWin_StatusIsWon()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (0,0) (1,1) (2,2) / O: (0,1) (0,2)
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(1, 1));
        game.Play(game.PlayerO, new Position(0, 2));
        game.Play(game.PlayerX, new Position(2, 2));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Contains(new Position(0, 0), game.WinningCells);
        Assert.Contains(new Position(1, 1), game.WinningCells);
        Assert.Contains(new Position(2, 2), game.WinningCells);
    }

    [Fact]
    public void Play_AntiDiagonalWin_StatusIsWon()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        // X: (0,2) (1,1) (2,0) / O: (0,0) (0,1)
        game.Play(game.PlayerX, new Position(0, 2));
        game.Play(game.PlayerO, new Position(0, 0));
        game.Play(game.PlayerX, new Position(1, 1));
        game.Play(game.PlayerO, new Position(0, 1));
        game.Play(game.PlayerX, new Position(2, 0));

        Assert.Equal(GameStatus.Won, game.Status);
        Assert.Contains(new Position(0, 2), game.WinningCells);
        Assert.Contains(new Position(1, 1), game.WinningCells);
        Assert.Contains(new Position(2, 0), game.WinningCells);
    }
}
