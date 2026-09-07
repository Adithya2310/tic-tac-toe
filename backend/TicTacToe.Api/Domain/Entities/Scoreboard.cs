namespace TicTacToe.Api.Domain;

/// <summary>
/// Session-level scoreboard tracking wins and draws across games.
/// Reset() explicitly zeroes all counts; individual game resets do not affect it.
/// </summary>
public sealed class Scoreboard
{
    public int XWins { get; private set; }
    public int OWins { get; private set; }
    public int Draws { get; private set; }

    /// <summary>Records a win for the given symbol. Only X and O are valid; Empty throws.</summary>
    public void RecordWin(Symbol symbol)
    {
        switch (symbol)
        {
            case Symbol.X: XWins++; break;
            case Symbol.O: OWins++; break;
            default: throw new ArgumentException($"Cannot record a win for symbol '{symbol}'.");
        }
    }

    public void RecordDraw() => Draws++;

    /// <summary>Resets all counters to zero.</summary>
    public void Reset()
    {
        XWins = 0;
        OWins = 0;
        Draws = 0;
    }
}
