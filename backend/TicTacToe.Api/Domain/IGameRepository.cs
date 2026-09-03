namespace TicTacToe.Api.Domain;

/// <summary>
/// Persistence contract for Game aggregates.
/// Implementations must not contain game-rule logic.
/// </summary>
public interface IGameRepository
{
    Game? GetById(Guid id);
    void Add(Game game);
    void Update(Game game);
    bool Exists(Guid id);
}
