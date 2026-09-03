namespace TicTacToe.Api.Domain;

/// <summary>
/// Represents one side in the game.
/// IsComputer distinguishes a human-controlled player from the computer player.
/// </summary>
public sealed class Player
{
    public Symbol Symbol { get; }
    public bool IsComputer { get; }

    public Player(Symbol symbol, bool isComputer)
    {
        Symbol = symbol;
        IsComputer = isComputer;
    }
}
