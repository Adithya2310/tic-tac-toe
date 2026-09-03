# Development Guidelines

## 1. General Principles

Prefer:

- simple code over clever code
- explicit names over abbreviations
- small classes with clear responsibilities
- deterministic behavior
- testable domain logic
- dependency inversion at external boundaries
- minimal coupling between layers

Avoid introducing abstractions without a concrete responsibility.

## 2. Naming

Use clear domain names:

```text
Game
Board
Move
Position
Scoreboard
StandardRules
BasicComputerMoveStrategy
GameFactory
GameService
ScoreboardService
```

Avoid vague names such as:

```text
Manager
Helper
Utility
Processor
Handler
Common
```

unless the responsibility is genuinely broad and clear.

## 3. Domain Rules

Game rules should be expressible without HTTP concepts.

Do not place business logic in:

- controllers
- DTOs
- repository implementations
- React components

## 4. Service Rules

A service should represent a meaningful application use case.

Good:

```text
CreateGame
GetGame
MakeMove
Undo
ResetGame
GetScoreboard
ResetScoreboard
```

Avoid services that merely wrap one method without adding a meaningful application boundary.

## 5. Dependency Injection

Use constructor injection.

Expected backend registrations include:

```text
IGameService -> GameService
IScoreboardService -> ScoreboardService

IGameRepository -> InMemoryGameRepository
IScoreboardRepository -> InMemoryScoreboardRepository

IGameFactory -> GameFactory
IComputerMoveStrategy -> BasicComputerMoveStrategy
IRules -> StandardRules
```

Choose appropriate lifetimes based on state ownership.

Because repositories contain session state, the initial in-memory implementation should have a lifetime that keeps state available for the application process.

## 6. Repository Rules

Repositories should answer persistence questions only:

```text
get
add
update
exists
reset
```

They should not:

- validate moves
- calculate wins
- select computer moves
- update scoreboard based on game rules

## 7. Exception and HTTP Boundary

Domain/application failures should be represented consistently.

The HTTP layer is responsible for converting those failures into HTTP responses.

Do not put `HttpResponseMessage`, controller types, or ASP.NET attributes into domain classes.

## 8. DTO Rules

Use request DTOs for inbound API data.

Use response DTOs for outbound API data.

Do not expose internal mutable collections directly.

The mapping should be explicit and easy to inspect.

## 9. Immutability and Encapsulation

Where practical:

- expose read-only collections
- keep setters private
- prevent callers from mutating game internals directly
- expose methods for valid state transitions

For example:

```text
Game.Play(...)
```

is preferable to exposing a mutable board setter.

## 10. Computer Strategy

`BasicComputerMoveStrategy` should be deterministic when a category contains multiple valid choices unless randomness is intentionally introduced and documented.

A straightforward deterministic order is preferred for:

- corners
- remaining cells

This makes tests stable and behavior easy to explain.

## 11. Testing

Tests should focus on observable behavior.

Example:

```text
Given an empty board
When X plays (0,0)
Then the board contains X at (0,0)
And O becomes the current player
And move history contains one move
```

Do not write tests that are tightly coupled to private implementation details.

## 12. Frontend Rules

React components should stay focused.

Recommended responsibilities:

### Game board

- render cells
- report cell clicks

### Game information

- current player
- mode
- completion message

### Move history

- render moves

### Scoreboard

- render scores

### Controls

- game mode
- reset
- undo
- scoreboard reset

### API service

- HTTP calls
- request/response typing
- centralized API URL configuration

React should not reimplement backend game rules.

## 13. Comments

Comments should explain intent, constraints, or non-obvious trade-offs.

Avoid comments that merely restate code.

Good:

```text
// Undo removes the human/computer pair so Computer Mode returns
// to the state immediately before the human's last turn.
```

Poor:

```text
// Remove the move.
```

## 14. Documentation

Update documentation when changing:

- API endpoints
- domain responsibilities
- design decisions
- assumptions
- test strategy
- run instructions

## 15. Git Practices

Prefer small commits with focused intent.

Examples:

```text
feat: add domain game model
feat: implement standard win and draw rules
feat: add in-memory game repository
feat: expose game REST endpoints
test: add game rule coverage
feat: add React game board
feat: integrate frontend with game API
docs: add architecture and API documentation
```

Avoid large commits that mix unrelated concerns.

## 16. AI-Generated Code Review Checklist

Before accepting AI-generated code, verify:

- Is the requirement actually present?
- Is the change in the correct layer?
- Is there duplicated business logic?
- Does it introduce unnecessary abstractions?
- Does it mutate state outside the domain boundary?
- Does it handle invalid inputs?
- Does it preserve backend source-of-truth behavior?
- Does it introduce hidden assumptions?
- Is there a test for important behavior?
- Does the implementation match the documented API?

AI-generated code must not be treated as reviewed code automatically.
