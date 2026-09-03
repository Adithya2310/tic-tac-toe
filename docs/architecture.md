# Architecture

## 1. Overview

The application is a browser-based Tic Tac Toe system with:

- React + TypeScript frontend
- .NET Web API backend
- REST API communication
- In-memory persistence
- Automated backend tests

The architecture separates HTTP concerns, application orchestration, domain behavior, and persistence.

```text
React Frontend
      |
      | HTTP/JSON
      v
Controllers
      |
      v
Application Services
      |
      v
Domain Model
      |
      v
Repository Interfaces
      |
      v
In-Memory Repositories
```

## 2. Layer Responsibilities

### Frontend

The React application is responsible for presentation and user interaction.

It should:

- display the board
- display current player
- display game mode
- display winner or draw state
- highlight winning cells
- display move history
- display the scoreboard
- enable/disable controls based on backend state
- call REST endpoints for game actions

It must not become the authoritative source of game rules.

### Controllers

Controllers expose the REST API.

Expected controllers:

- `GamesController`
- `ScoreboardController`

Controllers translate HTTP input to application service calls and service results to HTTP responses.

### Application Services

Expected services:

- `GameService`
- `ScoreboardService`

Services implement use cases and coordinate domain objects with repositories.

### Domain

The domain contains the Tic Tac Toe rules and behavior.

Core objects:

- `Game`
- `Board`
- `Player`
- `Move`
- `Position`
- `Scoreboard`
- `GameType`
- `GameStatus`
- `Symbol`
- `IRules`
- `StandardRules`
- `IComputerMoveStrategy`
- `BasicComputerMoveStrategy`
- `IGameFactory`
- `GameFactory`

### Repositories

Repositories isolate persistence.

Initial implementations:

- `InMemoryGameRepository`
- `InMemoryScoreboardRepository`

Repository code must not perform game-rule decisions.

## 3. Domain Relationships

```text
Game
 |
 +-- Board
 |
 +-- Player X
 |
 +-- Player O
 |
 +-- List<Move>
 |
 +-- IRules
 |
 +-- IComputerMoveStrategy (Computer mode only)
 |
 +-- GameType
 |
 +-- GameStatus
 |
 +-- CurrentPlayer
 |
 +-- Winner
 |
 +-- WinningCells
```

Factory relationship:

```text
GameFactory
     |
     +-- creates GameType.TwoPlayer
     |
     +-- creates GameType.Computer
              |
              +-- supplies computer move strategy
```

Rules relationship:

```text
IRules
  ^
  |
StandardRules
```

Strategy relationship:

```text
IComputerMoveStrategy
          ^
          |
BasicComputerMoveStrategy
```

Repository relationship:

```text
IGameRepository
      ^
      |
InMemoryGameRepository

IScoreboardRepository
      ^
      |
InMemoryScoreboardRepository
```

## 4. Why Board and Rules Are Shared Across Modes

Two-player mode and computer mode use the same 3 x 3 Tic Tac Toe board and the same fundamental rules for:

- valid cell placement
- row wins
- column wins
- diagonal wins
- draws

The difference between the modes is who controls O and how computer move selection and undo behavior operate.

Therefore the design does not create separate:

- `ComputerBoard`
- `TwoPlayerBoard`
- `ComputerRules`
- `TwoPlayerRules`

Doing so would duplicate unchanged behavior.

## 5. Why Strategy Is Used

Only computer move selection needs a distinct algorithmic behavior.

Human moves arrive from an API request.

Computer moves are selected by a strategy:

```text
IComputerMoveStrategy
        |
        v
BasicComputerMoveStrategy
        |
        v
Position
```

The strategy receives the current board and returns the selected position.

The initial strategy is intentionally stateless.

## 6. Why Factory Is Used

Game creation differs by `GameType`.

The factory centralizes construction so `GameService` does not contain detailed object-construction logic.

Conceptually:

```text
GameService
    |
    v
IGameFactory
    |
    v
GameFactory
    |
    +--> TwoPlayer Game
    |
    +--> Computer Game
              |
              +--> BasicComputerMoveStrategy
```

## 7. Why Repository Is Used

The assignment permits in-memory storage.

Repository interfaces are still useful because they keep persistence separate from application and domain logic.

Current:

```text
GameService
    |
    v
IGameRepository
    |
    v
InMemoryGameRepository
```

Possible future implementation:

```text
GameService
    |
    v
IGameRepository
    |
    v
EfGameRepository
    |
    v
EF Core / SQLite
```

No domain redesign should be necessary for that future replacement.

## 8. Undo Design

Undo is disabled after game completion.

For an in-progress game:

```text
TwoPlayer
    -> remove 1 move

Computer
    -> remove latest O move
    -> remove preceding X move
```

After removing moves, the board and status are reconstructed from the remaining valid move history.

This is preferable to maintaining a second mutable undo state for a small 3 x 3 game.

## 9. Scoreboard Ownership

The scoreboard is session-level backend state.

A completed game causes exactly one scoreboard update.

`Reset Game` does not change the scoreboard.

`Reset Scoreboard` explicitly clears scoreboard values.

The scoreboard is not calculated by the frontend.

## 10. Error Handling

Expected application-level failures include:

- game not found
- invalid move
- wrong player
- move outside the board
- occupied cell
- move after completion
- undo when no valid undo is available
- invalid game type

The controller/API boundary should translate these into consistent HTTP responses.

The domain should remain independent of HTTP concepts.

## 11. Non-Goals

The initial architecture intentionally excludes:

- authentication
- persistent multi-user identity
- external databases
- real-time sockets
- online multiplayer
- advanced AI
- distributed state
- message queues

These are outside the assignment scope.
