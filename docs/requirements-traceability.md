# Requirements Traceability

This document maps the assignment requirements to the planned implementation.

## 1. Product Requirements

| Requirement | Backend | Frontend / Test |
|---|---|---|
| 3 x 3 board | `Board` | Board component |
| Empty cells clickable | API validates cell; UI enables empty cells | Board component |
| X/O displayed | `Board` / `GameResponse` | Board component |
| Locked occupied cells | `Board` + `StandardRules` | Board component |
| Current player shown | `Game.CurrentPlayer` | Game information UI |
| Turn alternation | `Game.Play()` | State rendering |
| Invalid moves rejected | `Game` + `StandardRules` | API integration |
| Row win | `StandardRules` | Winning-cell rendering |
| Column win | `StandardRules` | Winning-cell rendering |
| Diagonal win | `StandardRules` | Winning-cell rendering |
| Winner displayed | `Game.Winner` | UI |
| Winning cells highlighted | `Game.WinningCells` | UI |
| Moves blocked after completion | `Game.Play()` | UI |
| Scoreboard updated once | `ScoreboardService` | UI |
| Draw detection | `StandardRules` | UI |
| Reset game | `Game.Reset()` | Reset control |
| Move history | `Move` + `Game.MoveHistory` | History component |
| Two-player undo | `Game.Undo()` | Undo control |
| Computer undo | `Game.Undo()` | Undo control |
| Scoreboard | `Scoreboard` + service/repository | Scoreboard component |
| Reset scoreboard | `ScoreboardService.Reset()` | Control |
| Computer mode | `GameType.Computer` | Mode selection |
| Human is X | `GameFactory` | UI |
| Computer is O | `GameFactory` | UI |
| Automatic computer move | `GameService` + strategy | UI observes returned state |
| Computer valid moves | `BasicComputerMoveStrategy` | Tests |
| No computer move after completion | `GameService` / `Game` | Tests |
| Required computer priority | `BasicComputerMoveStrategy` | Tests |

## 2. Backend API Requirements

| Capability | Endpoint |
|---|---|
| Create game | `POST /api/games` |
| Get game | `GET /api/games/{id}` |
| Submit move | `POST /api/games/{id}/moves` |
| Undo | `POST /api/games/{id}/undo` |
| Reset game | `POST /api/games/{id}/reset` |
| Get scoreboard | `GET /api/scoreboard` |
| Reset scoreboard | `POST /api/scoreboard/reset` |

## 3. Game State Contract

The game response exposes:

```text
Game ID
Board state
Current player
Game mode
Game status
Winner
Winning cells
Move history
```

The scoreboard is available through its dedicated endpoint.

## 4. Validation Matrix

| Invalid operation | Expected behavior |
|---|---|
| Row/column outside board | Reject |
| Occupied cell | Reject |
| Wrong player | Reject |
| Move after win | Reject |
| Move after draw | Reject |
| Undo with no moves | Reject / disabled |
| Undo after completion | Reject / disabled |

## 5. Undo Matrix

| Mode | Undo operation |
|---|---|
| TwoPlayer | Remove latest move |
| Computer | Remove latest O move and preceding X move |
| Completed game | Disabled |
| No moves | Disabled |

## 6. Scoreboard Rules

A completed game updates the scoreboard exactly once.

```text
X win -> XWins + 1
O win -> OWins + 1
Draw  -> Draws + 1
```

Reset Game does not modify scoreboard state.

Reset Scoreboard sets all values to zero.

## 7. Test Traceability

The backend test suite must cover at least:

```text
ValidMove
InvalidMove
TurnSwitching
RowWin
ColumnWin
DiagonalWin
Draw
ResetGame
UndoTwoPlayer
UndoComputer
ScoreboardUpdate
ComputerMoveSelection
MoveAfterGameCompletion
```

These tests collectively protect the highest-risk domain behavior.

## 8. Assignment and Repository Artifacts

Repository-level deliverables include:

- React frontend
- .NET backend
- tests
- README
- setup/run instructions
- test instructions
- API documentation
- design decisions
- assumptions and limitations
- AI-assisted workflow/prompt summary

The repository documentation should remain synchronized with implementation changes.
