using TicTacToe.Api.Api.Middleware;
using TicTacToe.Api.Application;
using TicTacToe.Api.Domain;
using TicTacToe.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------------
// Services
// ------------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS — allow the React dev server (Vite default: port 5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDevServer", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Domain
builder.Services.AddSingleton<IRules, StandardRules>();
builder.Services.AddSingleton<IComputerMoveStrategy, ComputerMoveStrategy>();
builder.Services.AddSingleton<IGameFactory, GameFactory>();

// Infrastructure — Singleton so in-memory state survives across requests.
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddSingleton<IScoreboardRepository, InMemoryScoreboardRepository>();

// Application services
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<ScoreboardService>();

// ------------------------------------------------------------------
// Pipeline
// ------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

// ---- Custom middleware — ORDER IS INTENTIONAL ----
//
// 1. Correlation: runs first so every layer below has the ID.
// 2. Logging: runs second so it can read the correlation ID AND see
//    the real response status after ExceptionHandling sets it.
// 3. ExceptionHandling: wraps the controller layer — catches domain
//    exceptions so controllers don't need try/catch.
//
app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("ReactDevServer");
app.MapControllers();

// Simple health probe.
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

app.Run();

// Expose Program for integration tests.
public partial class Program { }

