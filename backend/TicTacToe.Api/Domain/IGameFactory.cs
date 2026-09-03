namespace TicTacToe.Api.Domain;

/// <summary>
/// Creates fully initialised Game instances according to GameType.
/// The factory does not persist the new game; that is the service's responsibility.
/// </summary>
public interface IGameFactory
{
    Game Create(GameType gameType);
}
