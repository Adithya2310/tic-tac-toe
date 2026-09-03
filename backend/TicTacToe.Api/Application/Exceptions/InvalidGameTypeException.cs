namespace TicTacToe.Api.Application.Exceptions;

/// <summary>Raised when an unrecognised GameType string is received from the client.</summary>
public sealed class InvalidGameTypeException(string value)
    : Exception($"'{value}' is not a valid game type. Valid values: TwoPlayer, Computer.");
