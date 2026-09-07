using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Infrastructure;

/// <summary>
/// In-memory Scoreboard repository.
/// There is exactly one Scoreboard instance for the entire application session.
/// Registered as Singleton.
/// </summary>
public sealed class InMemoryScoreboardRepository : IScoreboardRepository
{
    private readonly Scoreboard _scoreboard = new();
    private readonly Lock _lock = new();

    public Scoreboard Get()
    {
        lock (_lock)
        {
            return _scoreboard;
        }
    }

    // The scoreboard is a mutable reference type stored in memory; mutations
    // made through Scoreboard methods are visible immediately. Save is provided
    // to satisfy the interface contract for future persistence-layer parity.
    public void Save(Scoreboard scoreboard)
    {
        // No-op for in-memory: the instance is shared by reference.
    }
}
