namespace TicTacToe.Api.Api.Middleware;

/// <summary>
/// Correlation middleware — runs first in the pipeline.
///
/// Responsibility:
///   Every HTTP request gets a unique Correlation ID.
///   If the incoming request already carries one (set by a client or a gateway),
///   that ID is reused. Otherwise a fresh UUID is generated.
///
/// The ID is stored in HttpContext.Items so every layer further down
/// the pipeline (logging, controllers, services) can read it without
/// knowing about HTTP headers.
///
/// The ID is also echoed back in the response header so the client
/// can quote it in a support request: "My request failed, here is the
/// X-Correlation-Id from the response."
/// </summary>
public sealed class CorrelationMiddleware
{
    /// <summary>
    /// The conventional header name used by most API gateways and clients.
    /// Both the incoming check and the outgoing echo use this same string.
    /// </summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>
    /// The key used to store the correlation ID inside HttpContext.Items.
    /// Other middleware and filters read it from here instead of from the header
    /// so the storage detail stays in one place.
    /// </summary>
    public const string ItemKey = "CorrelationId";

    private readonly RequestDelegate _next;

    // RequestDelegate is the function that calls the *next* middleware.
    // ASP.NET injects it automatically when you register with UseMiddleware<T>.
    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // --- STEP 1: Resolve the correlation ID ---
        // Check whether the client sent one; fall back to a new GUID.
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        // --- STEP 2: Publish it for the rest of this request's lifetime ---
        // HttpContext.Items is a per-request dictionary — safe to write here
        // and read anywhere downstream in the same request.
        context.Items[ItemKey] = correlationId;

        // --- STEP 3: Schedule writing it to the response header ---
        // We use OnStarting() because the response headers must be written
        // *before* the body starts. If we just set the header here, it might
        // be too late after await _next() has already flushed the response.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // --- STEP 4: Hand off to the rest of the pipeline ---
        await _next(context);
    }
}

