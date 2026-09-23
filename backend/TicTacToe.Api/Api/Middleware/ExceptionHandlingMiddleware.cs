using System.Text.Json;
using TicTacToe.Api.Application.Exceptions;

namespace TicTacToe.Api.Api.Middleware;

/// <summary>
/// Exception handling middleware — runs third in the pipeline, wrapping the
/// controller layer.
///
/// Responsibility:
///   Catch every unhandled exception from any layer (domain, application, or
///   infrastructure) and translate it into a consistent JSON error envelope:
///   { "code": "...", "message": "..." }
///
/// Why this matters for this project:
///   Without this middleware, GamesController has a try/catch block in every
///   single action method (GetGame, MakeMove, Undo, ResetGame) that all repeat
///   the same GameNotFoundException → 404 mapping. That is four copies of the
///   same logic. With this middleware, those try/catch blocks are removed and
///   the mapping lives in one place.
///
///   More importantly: any exception that a developer *forgets* to catch in a
///   controller currently returns a raw HTML error page or an empty 500, which
///   the React frontend cannot parse. This middleware guarantees that every
///   error — known or unknown — is always returned as JSON.
///
/// Running order:
///   This must be registered AFTER CorrelationMiddleware and LoggingMiddleware
///   so that:
///   - Correlation: the ID is in Items before we might log the error.
///   - Logging: the outgoing log line runs AFTER this middleware sets the
///     status code, so the log records the real 4xx/5xx that was sent.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    // JSON serialiser options reused for every error response.
    // camelCase matches the frontend's expectation (code, message).
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Hand off to the next middleware (ultimately the controller).
            // If nothing throws, execution continues normally past the try block.
            await _next(context);
        }

        // --- Known domain/application exceptions ---
        // These are expected business errors. We translate them to appropriate
        // HTTP status codes and structured error codes without logging a stack trace,
        // because they are not bugs — they are valid game-rule violations or
        // missing-resource lookups.

        catch (GameNotFoundException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, "GAME_NOT_FOUND", ex.Message);
        }
        catch (InvalidMoveException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "INVALID_MOVE", ex.Message);
        }
        catch (InvalidUndoException ex)
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "INVALID_UNDO", ex.Message);
        }

        // --- Unknown / unexpected exceptions ---
        // These ARE bugs. Log the full exception with stack trace so it shows
        // up in your logging system (console, Application Insights, Seq, etc.).
        // Return a generic 500 to the client — never expose internal details.
        catch (Exception ex)
        {
            var correlationId = context.Items[CorrelationMiddleware.ItemKey] as string ?? "none";

            _logger.LogError(
                ex,
                "[{CorrelationId}] Unhandled exception on {Method} {Path}",
                correlationId,
                context.Request.Method,
                context.Request.Path);

            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Writes a structured JSON error response and sets the HTTP status code.
    /// Called from every catch branch so the response format is always identical.
    /// </summary>
    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message)
    {
        // Setting StatusCode here is what LoggingMiddleware reads when it logs
        // the outgoing response line — so the log always shows the real status.
        context.Response.StatusCode  = statusCode;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { code, message }, JsonOptions);
        await context.Response.WriteAsync(body);
    }
}

