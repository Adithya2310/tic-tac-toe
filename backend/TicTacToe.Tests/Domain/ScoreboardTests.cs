using TicTacToe.Api.Domain;

namespace TicTacToe.Tests.Domain;

public sealed class ScoreboardTests
{
    [Fact]
    public void RecordWin_X_IncrementsXWins()
    {
        var scoreboard = new Scoreboard();

        scoreboard.RecordWin(Symbol.X);

        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    [Fact]
    public void RecordWin_O_IncrementsOWins()
    {
        var scoreboard = new Scoreboard();

        scoreboard.RecordWin(Symbol.O);

        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
    }

    [Fact]
    public void RecordDraw_IncrementsDraw()
    {
        var scoreboard = new Scoreboard();

        scoreboard.RecordDraw();

        Assert.Equal(1, scoreboard.Draws);
    }

    [Fact]
    public void Reset_ZeroesAllCounts()
    {
        var scoreboard = new Scoreboard();
        scoreboard.RecordWin(Symbol.X);
        scoreboard.RecordWin(Symbol.O);
        scoreboard.RecordDraw();

        scoreboard.Reset();

        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }
}
