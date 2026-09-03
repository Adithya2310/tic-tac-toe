# REST API Contract

## 1. Base Path

All game APIs are under:

```text
/api
```

## 2. Endpoint Summary

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/games` | Create a game |
| GET | `/api/games/{id}` | Get current game state |
| POST | `/api/games/{id}/moves` | Submit a player move |
| POST | `/api/games/{id}/undo` | Undo according to game mode |
| POST | `/api/games/{id}/reset` | Reset the current game |
| GET | `/api/scoreboard` | Get session scoreboard |
| POST | `/api/scoreboard/reset` | Reset session scoreboard |

The assignment permits endpoint names to vary; these names are the repository's chosen contract.

## 3. POST `/api/games`

### Request

```json
{
  "gameType": "TwoPlayer"
}
```

or:

```json
{
  "gameType": "Computer"
}
```

### Response

Returns the newly created game state.

Recommended status:

```text
201 Created
```

## 4. GET `/api/games/{id}`

Returns the current authoritative game state.

### Response

```json
{
  "gameId": "00000000-0000-0000-0000-000000000000",
  "board": [
    ["X", "", ""],
    ["", "O", ""],
    ["", "", ""]
  ],
  "currentPlayer": "X",
  "gameType": "TwoPlayer",
  "status": "InProgress",
  "winner": null,
  "winningCells": [],
  "moves": [
    {
      "moveNumber": 1,
      "player": "X",
      "row": 0,
      "column": 0
    },
    {
      "moveNumber": 2,
      "player": "O",
      "row": 1,
      "column": 1
    }
  ]
}
```

## 5. POST `/api/games/{id}/moves`

### Request

```json
{
  "player": "X",
  "row": 0,
  "column": 2
}
```

The game ID is supplied by the route.

### Processing

For TwoPlayer:

```text
validate request
-> load game
-> Game.Play()
-> save game
-> return state
```

For Computer:

```text
validate request
-> load game
-> Game.Play(X)
-> if game is still in progress:
     ComputerMoveStrategy.GetMove()
     Game.Play(O)
-> save game
-> return state
```

### Success

```text
200 OK
```

### Invalid move

Recommended:

```text
400 Bad Request
```

Examples:

- wrong player
- outside board
- occupied cell
- game already completed

## 6. POST `/api/games/{id}/undo`

### Request

No request body.

### Processing

Two-player:

```text
remove one move
```

Computer:

```text
remove computer move + preceding human move
```

### Success

```text
200 OK
```

Returns the updated `GameResponse`.

### Unavailable

Recommended:

```text
400 Bad Request
```

When:

- there are no moves to undo
- game is already completed
- computer mode does not have a complete human/computer pair to remove

## 7. POST `/api/games/{id}/reset`

### Request

No request body.

### Processing

Resets:

- board
- move history
- status
- winner
- winning cells
- current player

Does not change the scoreboard.

### Success

```text
200 OK
```

Returns the fresh game state.

## 8. GET `/api/scoreboard`

### Response

```json
{
  "xWins": 2,
  "oWins": 1,
  "draws": 3
}
```

## 9. POST `/api/scoreboard/reset`

Clears:

```text
xWins = 0
oWins = 0
draws = 0
```

Recommended status:

```text
200 OK
```

or `204 No Content` if the implementation chooses that convention consistently.

## 10. Error Shape

Use one consistent application error payload.

Recommended format:

```json
{
  "code": "INVALID_MOVE",
  "message": "The selected cell is already occupied."
}
```

Suggested codes:

```text
GAME_NOT_FOUND
INVALID_MOVE
WRONG_PLAYER
GAME_COMPLETED
NO_MOVES_TO_UNDO
INVALID_UNDO
INVALID_GAME_TYPE
```

Exact wording may vary, but error codes should remain stable.

## 11. DTO Separation

Do not expose domain entities directly as API contracts.

Recommended DTOs:

```text
CreateGameRequest
MakeMoveRequest
GameResponse
MoveResponse
ScoreboardResponse
```

This prevents HTTP concerns from leaking into the domain model and gives the API a stable boundary.

## 12. Frontend Consumption Rules

The React frontend should treat `GameResponse` as authoritative.

After a mutation endpoint returns a game state, React should render that returned state rather than independently reconstructing:

- winner
- turn
- move history
- board validity
- scoreboard outcome
