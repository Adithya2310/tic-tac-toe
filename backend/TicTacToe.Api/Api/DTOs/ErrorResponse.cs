namespace TicTacToe.Api.Api.DTOs;

/// <summary>
/// Consistent error envelope returned by all failed API calls.
/// Stable error codes allow clients to handle specific failures programmatically.
/// </summary>
public sealed class ErrorResponse
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public ErrorResponse(string code, string message)
    {
        Code = code;
        Message = message;
    }
}
