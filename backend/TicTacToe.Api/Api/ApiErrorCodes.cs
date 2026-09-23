namespace TicTacToe.Api.Api;

/// <summary>
/// Stable machine-readable error codes exposed by the HTTP API.
/// Keep these separate from user-facing exception messages so clients can make
/// decisions without parsing display text.
/// </summary>
public static class ApiErrorCodes
{
    public const string GameNotFound = "GAME_NOT_FOUND";
    public const string InvalidGameType = "INVALID_GAME_TYPE";
    public const string InvalidMove = "INVALID_MOVE";
    public const string InvalidUndo = "INVALID_UNDO";
    public const string InternalError = "INTERNAL_ERROR";
}
