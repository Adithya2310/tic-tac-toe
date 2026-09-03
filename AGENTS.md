# ABB Tic Tac Toe — AI Development Guidelines

## Purpose

This repository contains a take-home Tic Tac Toe application for ABB.

The application consists of:

- React + TypeScript frontend
- .NET Web API backend
- REST communication between frontend and backend
- In-memory persistence for the initial implementation
- Automated backend tests for core game behavior

The canonical product requirements are documented in the assignment supplied by ABB. Repository-specific architecture and implementation decisions are documented under `docs/`.

## Source of Truth

When making implementation decisions, use the following order of precedence:

1. `docs/requirements-traceability.md` — what must be implemented.
2. `docs/architecture.md` — how the application is structured.
3. `docs/domain-model.md` — domain responsibilities and boundaries.
4. `docs/api-contract.md` — HTTP contract between frontend and backend.
5. `docs/development-guidelines.md` — implementation conventions.

Do not introduce behavior that conflicts with the assignment requirements.

When a requirement is ambiguous, prefer the simplest implementation that satisfies the requirement and document the assumption rather than inventing additional product behavior.

## Core Architectural Rules

### Backend owns application truth

The backend is the source of truth for:

- board state
- current player
- game mode
- move history
- game status
- winner
- winning cells
- scoreboard

The frontend may keep local UI state, but it must render authoritative game state returned by the backend.

### Controllers are thin

Controllers should:

1. Receive HTTP requests.
2. Perform appropriate request/model validation.
3. Call the application service.
4. Translate the result or exception into an HTTP response.

Controllers must not contain:

- win detection
- draw detection
- turn switching
- computer move selection
- scoreboard mutation
- board mutation

### Services orchestrate use cases

Services coordinate domain operations and persistence.

`GameService` is responsible for game use cases.

`ScoreboardService` is responsible for scoreboard use cases.

Services should not duplicate domain rules that belong inside the domain model.

### Domain contains game behavior

The domain model owns Tic Tac Toe behavior.

`Game` coordinates a game turn and lifecycle.

`Board` manages board state.

`StandardRules` evaluates move legality, wins, and draws.

`BasicComputerMoveStrategy` selects the computer's move.

`GameFactory` constructs games according to `GameType`.

### Repository contains persistence concerns only

Repositories load and store domain state.

The initial implementation uses in-memory repositories.

Do not add Entity Framework Core or SQLite unless explicitly requested or intentionally approved as a later enhancement.

### Keep the initial implementation small

Do not add unrelated infrastructure such as:

- authentication
- authorization
- microservices
- message brokers
- caching layers
- external databases
- event sourcing
- CQRS
- Docker/Kubernetes

unless the assignment scope changes.

## Design Patterns

The intended patterns are:

- Factory: game construction based on `GameType`.
- Strategy: computer move selection.
- Repository: storage abstraction.
- Service layer: application/use-case orchestration.

Do not add a pattern solely to demonstrate pattern knowledge. Each abstraction should have a clear responsibility.

## Undo Policy

Use the assignment's allowed Option A:

- Undo is available only while the game is in progress.
- Undo is disabled after `Won` or `Draw`.
- The scoreboard is not rolled back.

Two-player mode removes one move.

Computer mode removes the computer's latest move together with the human's immediately preceding move.

## Persistence Policy

The first implementation is in-memory.

Repository interfaces must be designed so that a future EF Core/SQLite implementation could replace the in-memory implementation without changing the service/domain contracts.

Do not design the domain around a database schema.

## Computer Mode

Computer mode rules:

- Human is X.
- Computer is O.
- Computer moves automatically after a valid human move if the game is still in progress.
- The computer must never make a move after game completion.
- Move selection priority:
  1. O can win: take the winning move.
  2. X can win next: block X.
  3. Take center.
  4. Take a corner.
  5. Take any available cell.

## Testing Expectations

Backend tests are the primary test surface for game rules and state transitions.

At minimum, tests must cover:

- valid move
- invalid move
- turn switching
- row win
- column win
- diagonal win
- draw
- reset game
- undo in two-player mode
- undo in computer mode
- scoreboard update
- computer move selection
- move after game completion

## AI-Assisted Development

AI tools may be used during development.

Every generated change must be reviewed against:

- the ABB requirements
- the repository documentation
- existing tests
- architectural boundaries

Prefer small, verifiable changes.

Before committing generated code, ensure that the developer can explain:

- why the code exists
- which requirement it satisfies
- which layer owns it
- what assumptions were made
- what tests protect it

## Completion Standard

A feature is not considered complete merely because the code compiles.

A completed feature should have:

1. A clear requirement mapping.
2. Correct behavior.
3. Tests where applicable.
4. No unnecessary architectural coupling.
5. Updated documentation when behavior or API contracts change.
