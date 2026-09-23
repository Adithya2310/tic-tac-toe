using System.Diagnostics;

namespace TicTacToe.Api.Api.Middleware;

/// <summary>
/// Logging middleware — runs second in the pipeline, after CorrelationMiddleware.
///
/// Responsibility:
///   Logs every HTTP request and its response automatically, so no controller
///   or service needs to know it is being observed.
///
/// What it logs:
///   INCOMING:  [CorrelationId] → METHOD /path
///   OUTGOING:  [CorrelationId] ← METHOD /path STATUS in Xms
///
/// Running order matters:
///   Correlation must run first so the ID is in HttpContext.Items by the
///   time this middleware reads it.
///   Exception handling must run after this so that when an exception is
///   caught and turned into a 4xx/5xx, the response status is already set
///   by the time we log the outgoing line.
/// </summary>
public sealed class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    // ILogger<T> is injected by the DI container automatically.
    // The generic type parameter T makes the category name in the logs
    // match the class name, e.g. "TicTacToe.Api.Api.Middleware.LoggingMiddleware".
    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // --- STEP 1: Read the correlation ID set by CorrelationMiddleware ---
        // If it is missing (e.g. in tests where only this middleware is used),
        // fall back to "none" so the log line is still readable.
        var correlationId = context.Items[CorrelationMiddleware.ItemKey] as string ?? "none";

        var method = context.Request.Method;
        var path   = context.Request.Path;

        // --- STEP 2: Log the incoming request ---
        // The → arrow visually marks the start of a request, making it easy
        // to pair with the ← outgoing line when reading logs.
        _logger.LogInformation("[{CorrelationId}] → {Method} {Path}", correlationId, method, path);

        // --- STEP 3: Start a timer before handing off to the rest of the pipeline ---
        // Stopwatch is more accurate than DateTime.Now for elapsed time.
        var stopwatch = Stopwatch.StartNew();

        // --- STEP 4: Run the rest of the pipeline ---
        // Everything between "await _next(context)" below and the lines after it
        // is the time your controller + services actually spent on the request.
        await _next(context);

        stopwatch.Stop();

        // --- STEP 5: Log the outgoing response ---
        // By this point the ExceptionHandlingMiddleware has already caught any
        // exception and set context.Response.StatusCode to the correct value,
        // so we always log the real status that was returned to the client.
        var statusCode = context.Response.StatusCode;

        // Use LogWarning for 4xx (client errors) and LogError for 5xx (server errors)
        // so they show up in monitoring dashboards at the right severity level.
        var logLevel = statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _      => LogLevel.Information
        };

        _logger.Log(
            logLevel,
            "[{CorrelationId}] ← {Method} {Path} {StatusCode} in {ElapsedMs}ms",
            correlationId,
            method,
            path,
            statusCode,
            stopwatch.ElapsedMilliseconds);
    }
}

