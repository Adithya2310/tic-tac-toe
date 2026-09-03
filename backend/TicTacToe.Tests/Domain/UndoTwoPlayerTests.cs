using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class UndoTwoPlayerTests
{
    [Fact]
    public void Undo_RemovesLastMove()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 1));

        game.Undo();

        Assert.Single(game.MoveHistory);
        Assert.Equal(Symbol.Empty, game.Board.GetCell(new Position(1, 1)));
    }

    [Fact]
    public void Undo_RestoresCurrentPlayer()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0)); // after this: O's turn
        game.Play(game.PlayerO, new Position(1, 1)); // after this: X's turn

        game.Undo(); // remove O's move → back to O's turn

        Assert.Equal(Symbol.O, game.CurrentPlayer.Symbol);
    }

    [Fact]
    public void Undo_WithNoMoves_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();

        Assert.Throws<InvalidOperationException>(() => game.Undo());
    }

    [Fact]
    public void Undo_AfterGameCompleted_Throws()
    {
        var game = GameTestFactory.CreateTwoPlayerGame();
        game.Play(game.PlayerX, new Position(0, 0));
        game.Play(game.PlayerO, new Position(1, 0));
        game.Play(game.PlayerX, new Position(0, 1));
        game.Play(game.PlayerO, new Position(1, 1));
        game.Play(game.PlayerX, new Position(0, 2)); // X wins

        Assert.Throws<InvalidOperationException>(() => game.Undo());
    }
}
