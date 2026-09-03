namespace TicTacToe.Api.Application.Exceptions;

/// <summary>Raised when a game with the requested ID does not exist.</summary>
public sealed class GameNotFoundException(Guid gameId)
    : Exception($"Game '{gameId}' was not found.");
