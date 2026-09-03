using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Api.DTOs;
using TicTacToe.Api.Application;
using TicTacToe.Api.Application.Exceptions;
using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Api.Controllers;

[ApiController]
[Route("api/games")]
public sealed class GamesController : ControllerBase
{
    private readonly GameService _gameService;

    public GamesController(GameService gameService)
    {
        _gameService = gameService;
    }

    // POST /api/games
    [HttpPost]
    [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public IActionResult CreateGame([FromBody] CreateGameRequest request)
    {
        if (!Enum.TryParse<GameType>(request.GameType, ignoreCase: true, out var gameType))
            return BadRequest(new ErrorResponse("INVALID_GAME_TYPE",
                $"'{request.GameType}' is not a valid game type. Valid values: TwoPlayer, Computer."));

        var game = _gameService.CreateGame(gameType);
        var response = GameMapper.ToResponse(game);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, response);
    }

    // GET /api/games/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult GetGame(Guid id)
    {
        try
        {
            var game = _gameService.GetGame(id);
            return Ok(GameMapper.ToResponse(game));
        }
        catch (GameNotFoundException ex)
        {
            return NotFound(new ErrorResponse("GAME_NOT_FOUND", ex.Message));
        }
    }

    // POST /api/games/{id}/moves
    [HttpPost("{id:guid}/moves")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult MakeMove(Guid id, [FromBody] MakeMoveRequest request)
    {
        if (!Enum.TryParse<Symbol>(request.Player, ignoreCase: true, out var symbol) || symbol == Symbol.Empty)
            return BadRequest(new ErrorResponse("INVALID_MOVE",
                $"'{request.Player}' is not a valid player symbol. Valid values: X, O."));

        try
        {
            var game = _gameService.MakeMove(id, symbol, new Position(request.Row, request.Column));
            return Ok(GameMapper.ToResponse(game));
        }
        catch (GameNotFoundException ex)
        {
            return NotFound(new ErrorResponse("GAME_NOT_FOUND", ex.Message));
        }
        catch (InvalidMoveException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_MOVE", ex.Message));
        }
    }

    // POST /api/games/{id}/undo
    [HttpPost("{id:guid}/undo")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult Undo(Guid id)
    {
        try
        {
            var game = _gameService.Undo(id);
            return Ok(GameMapper.ToResponse(game));
        }
        catch (GameNotFoundException ex)
        {
            return NotFound(new ErrorResponse("GAME_NOT_FOUND", ex.Message));
        }
        catch (InvalidUndoException ex)
        {
            return BadRequest(new ErrorResponse("INVALID_UNDO", ex.Message));
        }
    }

    // POST /api/games/{id}/reset
    [HttpPost("{id:guid}/reset")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult ResetGame(Guid id)
    {
        try
        {
            var game = _gameService.ResetGame(id);
            return Ok(GameMapper.ToResponse(game));
        }
        catch (GameNotFoundException ex)
        {
            return NotFound(new ErrorResponse("GAME_NOT_FOUND", ex.Message));
        }
    }
}
