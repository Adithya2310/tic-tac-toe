# Domain Model

## 1. Purpose

This document defines the backend domain model and the responsibility of each domain type.

The objective is to keep Tic Tac Toe rules testable and independent from HTTP and persistence concerns.

## 2. Enumerations

### Symbol

```text
Empty
X
O
```

Represents the content of a board cell or a player's symbol.

### GameType

```text
TwoPlayer
Computer
```

Represents the selected game mode.

### GameStatus

```text
InProgress
Won
Draw
```

Represents the lifecycle state of a game.

## 3. Position

Recommended representation:

```text
Position
- Row
- Column
```

Responsibility:

- represent a single board coordinate
- provide a common type for moves and winning cells

It does not own game logic.

## 4. Move

`Move` represents a completed move in game history.

State:

```text
MoveNumber
Player
Position
```

Behavior:

None beyond value/data semantics.

Important distinction:

`Move` records what happened.

`Game.Play()` performs the move.

## 5. Player

State:

```text
Symbol
IsComputer
```

Expected configuration:

### Two-player mode

```text
Player X -> IsComputer = false
Player O -> IsComputer = false
```

### Computer mode

```text
Player X -> IsComputer = false
Player O -> IsComputer = true
```

The player object does not decide a move.

## 6. Board

State:

```text
3 x 3 grid of Symbol
```

Core functions:

```text
IsValidPosition(Position)
IsCellEmpty(Position)
PlaceMark(Position, Symbol)
GetCell(Position)
GetEmptyPositions()
IsFull()
Clear()
Clone()
```

Responsibilities:

- own the current board contents
- enforce cell-level constraints
- expose board information to rules and strategy

Not responsible for:

- turn management
- winner ownership
- scoreboard updates
- HTTP requests
- persistence

## 7. Rules

Interface:

```text
IRules

IsValidMove(Board, Symbol, Position)
CheckWin(Board, Symbol) -> winning positions
CheckDraw(Board) -> bool
```

`StandardRules` implements the interface.

### IsValidMove

Checks board-level move legality such as:

- position is inside the board
- target cell is empty

Current-player validation is a game lifecycle concern and is handled by `Game`.

### CheckWin

Checks:

- all rows
- all columns
- both diagonals

When a win exists, return the winning cells.

### CheckDraw

Returns true when:

- the board is full
- there is no winner

## 8. Computer Move Strategy

Interface:

```text
IComputerMoveStrategy

GetMove(Board) -> Position
```

Initial implementation:

```text
BasicComputerMoveStrategy
```

State:

- no mutable game state is required

Behavior:

1. Check whether O has a winning move.
2. Otherwise check whether X has a winning move and block it.
3. Otherwise choose the center if available.
4. Otherwise choose an available corner.
5. Otherwise choose any available cell.

The strategy should never:

- mutate the real game directly
- update the scoreboard
- persist the game
- decide HTTP responses

It only selects a position.

## 9. Game

`Game` is the central domain aggregate.

State:

```text
Id
GameType
Board
PlayerX
PlayerO
CurrentPlayer
GameStatus
Winner
WinningCells
MoveHistory
Rules
ComputerMoveStrategy (optional)
```

Core behavior:

```text
Play(Player, Position)
Undo()
Reset()
```

Recommended internal helpers:

```text
IsCompleted()
SwitchTurn()
RebuildBoardFromMoves()
RecalculateState()
```

### Play

The operation should:

1. Reject a move when the game is complete.
2. Reject a move from the wrong player.
3. Validate the board position through `IRules`.
4. Place the symbol on the board.
5. Create a `Move`.
6. Add the move to history.
7. Check for a winner.
8. Otherwise check for a draw.
9. Otherwise switch the current player.

### Undo

Only permitted while the game is in progress.

Two-player:

```text
remove latest move
```

Computer:

```text
remove latest computer move
remove preceding human move
```

Then:

1. clear the board
2. replay remaining moves
3. recalculate game state
4. restore the correct current player

### Reset

Reset only the current game:

```text
Board -> empty
Move history -> empty
Status -> InProgress
Winner -> null
Winning cells -> empty
Current player -> X
```

The scoreboard is not changed.

## 10. Scoreboard

State:

```text
XWins
OWins
Draws
```

Behavior:

```text
RecordWin(Symbol)
RecordDraw()
Reset()
```

Scoreboard logic is session-level application state, not part of an individual game's board state.

## 11. Game Factory

Interface:

```text
IGameFactory

Create(GameType) -> Game
```

`GameFactory` owns game construction.

Expected construction behavior:

### TwoPlayer

Creates a game with:

- X human
- O human
- `StandardRules`
- no computer strategy

### Computer

Creates a game with:

- X human
- O computer
- `StandardRules`
- `BasicComputerMoveStrategy`

The factory should not persist the new game.

## 12. Domain Boundaries

The domain must not depend on:

- ASP.NET Core controllers
- HTTP request/response types
- EF Core
- repository implementations
- React/frontend types

The domain should be unit-testable without running the web server.
