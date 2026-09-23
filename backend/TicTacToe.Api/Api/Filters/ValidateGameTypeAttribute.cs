using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TicTacToe.Api.Api.DTOs;
using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Api.Filters;

/// <summary>
/// Action filter that validates the GameType string in a CreateGameRequest
/// BEFORE the action body runs.
///
/// Applied as [ValidateGameType] on the CreateGame action.
/// Short-circuits with 400 INVALID_GAME_TYPE if the value is unrecognised.
/// Stores the parsed GameType enum in HttpContext.Items so the controller
/// can use it directly without calling Enum.TryParse again.
///
/// WHY A FILTER AND NOT MIDDLEWARE?
///   At the time middleware runs, the HTTP request body is a raw byte stream —
///   request.GameType doesn't exist yet as a C# string. Model binding hasn't
///   happened. The filter runs AFTER model binding, so it receives the already-
///   constructed CreateGameRequest object via ActionExecutingContext.ActionArguments.
///   Middleware simply cannot access the bound model at all.
///
///   Additionally, this validation only applies to CreateGame. A middleware
///   would run for every single HTTP request, including health checks and
///   static files — that's the wrong scope.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateGameTypeAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Key used to store the parsed enum in HttpContext.Items.
    /// The controller reads it from here after the filter has validated.
    /// </summary>
    public const string ItemKey = "ParsedGameType";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // ActionExecutingContext.ActionArguments is a dictionary of every bound
        // parameter of the action method, keyed by the parameter name.
        // "request" matches the parameter name:  CreateGame([FromBody] CreateGameRequest request)
        if (!context.ActionArguments.TryGetValue("request", out var obj)
            || obj is not CreateGameRequest request)
        {
            // This filter was mis-applied to an action without a CreateGameRequest.
            // Fail open — don't block the request for an unrelated reason.
            return;
        }

        if (!Enum.TryParse<GameType>(request.GameType, ignoreCase: true, out var gameType))
        {
            // Setting context.Result short-circuits the pipeline.
            // The actual controller action method NEVER executes when Result is set here.
            // This is the filter equivalent of the controller returning BadRequest().
            context.Result = new BadRequestObjectResult(new ErrorResponse(
                "INVALID_GAME_TYPE",
                $"'{request.GameType}' is not a valid game type. Valid values: TwoPlayer, Computer."));
            return;
        }

        // Store the successfully parsed GameType so the controller can read it
        // from HttpContext.Items without parsing again — single-parse, no duplication.
        context.HttpContext.Items[ItemKey] = gameType;
    }
}

