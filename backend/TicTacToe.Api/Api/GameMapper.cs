using TicTacToe.Api.Api.DTOs;
using TicTacToe.Api.Domain;

namespace TicTacToe.Api.Api;

/// <summary>
/// Maps domain objects to API response DTOs.
/// Keeps mapping logic explicit and out of controllers.
/// </summary>
internal static class GameMapper
{
    public static GameResponse ToResponse(Game game)
    {
        var grid = game.Board.ToGrid();

        return new GameResponse
        {
            GameId = game.Id,
            Board = grid.Select(row =>
                row.Select(cell => cell == Symbol.Empty ? "" : cell.ToString()).ToList()
            ).ToList(),
            CurrentPlayer = game.CurrentPlayer.Symbol.ToString(),
            GameType = game.GameType.ToString(),
            Status = game.Status.ToString(),
            Winner = game.Winner?.Symbol.ToString(),
            WinningCells = game.WinningCells.Select(p => new WinningCellResponse
            {
                Row = p.Row,
                Column = p.Column
            }).ToList(),
            Moves = game.MoveHistory.Select(m => new MoveResponse
            {
                MoveNumber = m.MoveNumber,
                Player = m.Player.Symbol.ToString(),
                Row = m.Position.Row,
                Column = m.Position.Column
            }).ToList()
        };
    }

    public static ScoreboardResponse ToResponse(Scoreboard scoreboard) => new()
    {
        XWins = scoreboard.XWins,
        OWins = scoreboard.OWins,
        Draws = scoreboard.Draws
    };
}
