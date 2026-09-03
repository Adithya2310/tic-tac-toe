using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Infrastructure;

/// <summary>
/// In-memory Game repository backed by a thread-safe dictionary.
/// Registered as Singleton so game state survives across requests.
/// </summary>
public sealed class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<Guid, Game> _store = [];
    private readonly Lock _lock = new();

    public Game? GetById(Guid id)
    {
        lock (_lock)
        {
            _store.TryGetValue(id, out var game);
            return game;
        }
    }

    public void Add(Game game)
    {
        lock (_lock)
        {
            _store[game.Id] = game;
        }
    }

    // Games are stored by reference, so mutations made to the Game object
    // through domain methods (Play, Undo, Reset) are visible without
    // an explicit Update call. Update is provided to satisfy the interface
    // contract and to make future EF Core implementation easier.
    public void Update(Game game)
    {
        lock (_lock)
        {
            _store[game.Id] = game;
        }
    }

    public bool Exists(Guid id)
    {
        lock (_lock)
        {
            return _store.ContainsKey(id);
        }
    }
}
