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
builder.Services.AddSingleton<IComputerMoveStrategy, BasicComputerMoveStrategy>();
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

app.UseCors("ReactDevServer");

app.MapControllers();

// Simple health probe.
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

app.Run();

// Expose Program for integration tests.
public partial class Program { }
