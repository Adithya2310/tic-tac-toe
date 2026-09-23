using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Api.DTOs;
using TicTacToe.Api.Api.Filters;
using TicTacToe.Api.Application;
using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Api.Controllers;

/// <summary>
/// Thin HTTP adapter. Responsibilities:
///   1. Parse and validate the HTTP request.
///   2. Call the application service.
///   3. Return the appropriate HTTP response.
///
/// Exception handling is NOT the controller's responsibility.
/// ExceptionHandlingMiddleware catches every domain/application exception
/// and maps it to the correct HTTP status code and JSON error body.
/// That is why there are no try/catch blocks here.
/// </summary>
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
    // [ValidateGameType] runs BEFORE this method body.
    // If the GameType string is invalid, the filter short-circuits with a 400
    // and this method never executes.
    // If it is valid, the parsed enum is waiting in HttpContext.Items.
    [HttpPost]
    [ValidateGameType]
    [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public IActionResult CreateGame([FromBody] CreateGameRequest request)
    {
        // The filter already validated and parsed GameType — read it directly.
        // The cast is safe because [ValidateGameType] guarantees the key exists here.
        var gameType = (GameType)HttpContext.Items[ValidateGameTypeAttribute.ItemKey]!;

        if (request.BoardSize < 3)
            return BadRequest(new ErrorResponse("INVALID_BOARD_SIZE",
                $"Board size must be at least 3. Received: {request.BoardSize}."));

        var game = _gameService.CreateGame(gameType, request.BoardSize);
        var response = GameMapper.ToResponse(game);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, response);
    }


    // GET /api/games/{id}
    // GameNotFoundException → 404 is handled by ExceptionHandlingMiddleware.
    [HttpGet("{id:guid}")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult GetGame(Guid id)
    {
        var game = _gameService.GetGame(id);
        return Ok(GameMapper.ToResponse(game));
    }

    // POST /api/games/{id}/moves
    // GameNotFoundException → 404, InvalidMoveException → 400
    // are both handled by ExceptionHandlingMiddleware.
    [HttpPost("{id:guid}/moves")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult MakeMove(Guid id, [FromBody] MakeMoveRequest request)
    {
        if (!Enum.TryParse<Symbol>(request.Player, ignoreCase: true, out var symbol) || symbol == Symbol.Empty)
            return BadRequest(new ErrorResponse("INVALID_MOVE",
                $"'{request.Player}' is not a valid player symbol. Valid values: X, O."));

        var game = _gameService.MakeMove(id, symbol, new Position(request.Row, request.Column));
        return Ok(GameMapper.ToResponse(game));
    }

    // POST /api/games/{id}/undo
    // GameNotFoundException → 404, InvalidUndoException → 400
    // are both handled by ExceptionHandlingMiddleware.
    [HttpPost("{id:guid}/undo")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult Undo(Guid id)
    {
        var game = _gameService.Undo(id);
        return Ok(GameMapper.ToResponse(game));
    }

    // POST /api/games/{id}/reset
    // GameNotFoundException → 404 is handled by ExceptionHandlingMiddleware.
    [HttpPost("{id:guid}/reset")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public IActionResult ResetGame(Guid id)
    {
        var game = _gameService.ResetGame(id);
        return Ok(GameMapper.ToResponse(game));
    }
}
