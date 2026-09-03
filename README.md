# Tic Tac Toe — ABB Take-Home Assignment

A full-stack Tic Tac Toe application built with **React + TypeScript** (frontend) and **.NET 10 Web API** (backend), communicating via REST/JSON.

---

## Table of Contents

1. [Project Structure](#project-structure)
2. [Requirements](#requirements)
3. [Setup & Running](#setup--running)
4. [API Reference](#api-reference)
5. [Design Decisions](#design-decisions)
6. [Assumptions & Limitations](#assumptions--limitations)
7. [AI-Assisted Workflow](#ai-assisted-workflow)
8. [Implementation Progress](#implementation-progress)

---

## Project Structure

```
TicTacToe/
├── backend/
│   ├── TicTacToe.Api/          # ASP.NET Core Web API
│   │   ├── Api/
│   │   │   ├── Controllers/    # GamesController, ScoreboardController
│   │   │   └── DTOs/           # Request/Response DTOs
│   │   ├── Application/        # GameService, ScoreboardService
│   │   │   └── Exceptions/     # Domain exception types
│   │   ├── Domain/             # Game, Board, Rules, Strategy, Factory
│   │   ├── Infrastructure/     # In-memory repositories
│   │   └── Program.cs
│   └── TicTacToe.Tests/        # xUnit backend tests
├── frontend/                   # React + TypeScript (Vite) — coming soon
├── docs/
│   ├── architecture.md
│   ├── development-guidelines.md
│   ├── requirements-traceability.md
│   └── backend/
│       ├── api-contract.md
│       └── domain-model.md
├── AGENTS.md
├── TicTacToe.slnx
└── README.md
```

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (for frontend, added in a later milestone)

---

## Setup & Running

### Backend

```bash
# From the repository root
dotnet run --project backend/TicTacToe.Api
```

The API server starts at **http://localhost:5000**.

Swagger UI (development only): **http://localhost:5000/openapi/v1.json**

### Health Check

```bash
curl http://localhost:5000/health
# {"status":"healthy","timestamp":"..."}
```

### Tests

```bash
dotnet test
```

### Frontend

> Coming in Milestone 6.

---

## API Reference

> Full contract: [`docs/backend/api-contract.md`](docs/backend/api-contract.md)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/games` | Create a new game |
| GET | `/api/games/{id}` | Get current game state |
| POST | `/api/games/{id}/moves` | Submit a player move |
| POST | `/api/games/{id}/undo` | Undo the last move(s) |
| POST | `/api/games/{id}/reset` | Reset the board |
| GET | `/api/scoreboard` | Get session scoreboard |
| POST | `/api/scoreboard/reset` | Reset scoreboard to zero |

---

## Design Decisions

### Layered architecture

Controllers → Services → Domain → Repositories, with strict dependency direction. The domain has no ASP.NET or EF dependencies, making it independently unit-testable.

### In-memory persistence

The initial implementation stores games and the scoreboard in memory (singleton lifetime). Repository interfaces (`IGameRepository`, `IScoreboardRepository`) are designed so an EF Core/SQLite implementation could replace the in-memory one without changing any service or domain code.

### Backend as single source of truth

All game state (board, current player, winner, winning cells, scoreboard) lives on the backend. The React frontend renders what the API returns — it does not recompute any game rules.

### Undo design (Option A)

- Undo is available only while the game is `InProgress`.
- Two-Player mode removes one move.
- Computer mode removes the computer's latest move and the immediately preceding human move.
- The board is rebuilt by replaying the remaining move history (avoids maintaining a separate undo stack).
- Scoreboard is not rolled back on undo.

### Computer move priority

`BasicComputerMoveStrategy` evaluates positions in this order:
1. Winning move for O
2. Blocking move for X
3. Center (1,1)
4. Any corner (deterministic order: top-left, top-right, bottom-left, bottom-right)
5. Any remaining cell

Deterministic ordering makes the strategy predictable and the tests stable.

### Error responses

All domain/application errors are converted to a consistent error envelope:

```json
{ "code": "INVALID_MOVE", "message": "..." }
```

---

## Assumptions & Limitations

- **Single session**: The in-memory store is process-scoped. Restarting the server clears all games.
- **No authentication**: Any client can access any game by ID. This is acceptable for the assignment scope.
- **Single active game per session**: The frontend manages one game at a time, but the API supports multiple concurrent games.
- **Computer move is synchronous**: The computer move is computed and applied within the same HTTP request as the human move — no polling or websockets needed.

---

## AI-Assisted Workflow

This project was built with AI assistance (Google Antigravity / Gemini). The AI was used to:

- Review and interpret the assignment requirements
- Generate an 8-milestone implementation plan
- Scaffold the solution structure
- Generate domain model, service, and controller code
- Write backend tests
- Generate React component scaffolds

Every generated change was reviewed against the documented requirements, architecture decisions, and layer boundaries (see `AGENTS.md`) before being committed.

Prompts were kept focused on one milestone at a time. Generated code that introduced behaviors not in the requirements was rejected or removed.

---

## Implementation Progress

| Milestone | Description | Status |
|-----------|-------------|--------|
| 1 | Backend scaffold (solution, projects, CORS, health endpoint) | ✅ Complete |
| 2 | Domain model (Game, Board, Rules, Strategy, Factory) | ✅ Complete |
| 3 | Repositories + Services | ✅ Complete |
| 4 | REST Controllers + DTOs | ✅ Complete |
| 5 | Backend tests (38 tests, all passing) | ✅ Complete |
| 6 | React frontend scaffold + API service | 🔄 In progress |
| 7 | React UI components | ⬜ Pending |
| 8 | Polish + README finalization | ⬜ Pending |
