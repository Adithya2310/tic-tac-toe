using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Api.DTOs;
using TicTacToe.Api.Application;

namespace TicTacToe.Api.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public sealed class ScoreboardController : ControllerBase
{
    private readonly ScoreboardService _scoreboardService;

    public ScoreboardController(ScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    // GET /api/scoreboard
    [HttpGet]
    [ProducesResponseType<ScoreboardResponse>(StatusCodes.Status200OK)]
    public IActionResult GetScoreboard()
    {
        var scoreboard = _scoreboardService.GetScoreboard();
        return Ok(GameMapper.ToResponse(scoreboard));
    }

    // POST /api/scoreboard/reset
    [HttpPost("reset")]
    [ProducesResponseType<ScoreboardResponse>(StatusCodes.Status200OK)]
    public IActionResult ResetScoreboard()
    {
        var scoreboard = _scoreboardService.ResetScoreboard();
        return Ok(GameMapper.ToResponse(scoreboard));
    }
}
