using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class UndoComputerTests
{
    /// <summary>
    /// In Computer mode, Undo removes both O's (computer) move and the preceding X (human) move.
    /// After undo the board returns to the state before the human's last turn.
    /// </summary>
    [Fact]
    public void Undo_Computer_RemovesHumanAndComputerMove()
    {
        var game = GameTestFactory.CreateComputerGame();

        // X plays (0,0); computer responds automatically via Play.
        // We drive the game manually here (without GameService) to control positions.
        game.Play(game.PlayerX, new Position(0, 0));
        // Computer (O) is not auto-played by the domain — that's the service's job.
        // We simulate the service by calling Play(PlayerO) ourselves.
        game.Play(game.PlayerO, new Position(1, 1));

        game.Undo();

        // Both moves should be removed.
        Assert.Empty(game.MoveHistory);
        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(0, 0)));
        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(1, 1)));
        Assert.Equal(Symbol.X, game.CurrentPlayer.Symbol);
    }

    [Fact]
    public void Undo_Computer_SingleHumanMove_RemovesThatMove()
    {
        // Edge case: only X has moved, computer has not yet responded.
        var game = GameTestFactory.CreateComputerGame();
        game.Play(game.PlayerX, new Position(0, 0));

        game.Undo();

        Assert.Empty(game.MoveHistory);
        Assert.Equal(Symbol.X, game.CurrentPlayer.Symbol);
    }

    [Fact]
    public void Undo_Computer_MultiplePairs_RemovesOnlyLatestPair()
    {
        var game = GameTestFactory.CreateComputerGame();

        // First pair
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 1));

        // Second pair
        game.Play(game.PlayerX, new Position(0, 1));
        game.Play(game.PlayerO, new Position(2, 2));

        game.Undo(); // should remove second pair only

        Assert.Equal(2, game.MoveHistory.Count);
        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(0, 1)));
        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(2, 2)));
        Assert.Equal(Symbol.X, game.Board.GetCell(new Position(0, 0)));
        Assert.Equal(Symbol.O, game.Board.GetCell(new Position(1, 1)));
    }
}
