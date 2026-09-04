# Tic Tac Toe — AI Development Guidelines



## Purpose



This repository contains a take-home Tic Tac Toe application for a software engineering assignment.



The application consists of:



- React + TypeScript frontend



- .NET Web API backend



- REST communication between frontend and backend



- In-memory persistence for the initial implementation



- Automated backend tests for core game behavior



The canonical product requirements are documented in the assignment. Repository-specific architecture and implementation decisions are documented under `docs/`.



## Source of Truth



When making implementation decisions, use the following order of precedence:



1. `docs/requirements-traceability.md` — what must be implemented.



2. `docs/architecture.md` — how the application is structured.



3. `docs/domain-model.md` — domain responsibilities and boundaries.



4. `docs/api-contract.md` — HTTP contract between frontend and backend.



5. `docs/development-guidelines.md` — implementation conventions.



6. `wireframes/` — approved frontend visual references and design specifications.



7. `docs/frontend-guidelines.md` — frontend implementation conventions and best practices.



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



- the assignment requirements



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



---



# Frontend Development Guidelines



The following rules are additive to the backend and application guidelines above. They do not replace or weaken any existing backend architecture, domain, persistence, testing, or design-pattern rules.



## Frontend Scope



The frontend is a React + TypeScript application with exactly two application routes:



- `/` — Home / game mode selection



- `/game/:gameId` — active game session



The game-over experience is a modal state on `/game/:gameId`, not a separate route.



The frontend should implement the required game experience without introducing unrelated product areas such as:



- leaderboard



- historical game archive



- user profiles



- authentication



- settings



- analytics dashboard



- API documentation screens



- unrelated navigation



## Frontend Architecture



Use a lightweight React architecture appropriate for a small application.



A recommended structure is:



```text
src/
├── components/
│   ├── ui/
│   ├── home/
│   └── game/
├── pages/
│   ├── HomePage.tsx
│   └── GamePage.tsx
├── hooks/
│   └── useGame.ts
├── services/
│   └── gameApi.ts
├── types/
│   └── game.ts
├── lib/
│   └── utils.ts
├── styles/
│   └── index.css
├── App.tsx
└── main.tsx
```



Do not create abstractions, folders, or libraries solely to demonstrate architectural knowledge.



## React Component Responsibilities



Use focused components with clear responsibilities.



Typical components include:



- `Header`
- `GameModeSelector`
- `GameStatus`
- `GameBoard`
- `BoardCell`
- `PlayerPanel`
- `MoveActivity`
- `Scoreboard`
- `GameControls`
- `GameOverModal`



Pages should primarily compose components and coordinate page-level behavior.



Presentational components should not directly own API communication.



## Backend Is the Source of Truth



The frontend must not become a second Tic Tac Toe engine.



Do not duplicate authoritative backend logic in React for:



- move validation



- turn switching



- win detection



- draw detection



- winning-cell calculation



- computer move selection



- scoreboard calculation



The frontend sends user intent to the backend and renders the returned authoritative state.



The preferred flow is:



```text
User interaction
      ↓
React component
      ↓
useGame()
      ↓
gameApi.ts
      ↓
.NET REST API
      ↓
authoritative response
      ↓
React state
      ↓
UI
```



Local React state is appropriate for UI-only concerns such as:



- loading indicators



- request-in-progress state



- modal visibility



- temporary error presentation



## Frontend API Boundary



All backend communication should be centralized in a dedicated API/service module.



For example:



```text
src/services/gameApi.ts
```



Typical operations include:



```text
createGame(mode)
getGame(gameId)
makeMove(gameId, row, column)
undo(gameId)
resetGame(gameId)
getScoreboard()
resetScoreboard()
```



Components should not contain repeated raw `fetch()` calls or HTTP URL construction.



The exact endpoints, payloads, and response types must follow `docs/api-contract.md`.



## State Management



Use the smallest state-management solution that fits the application.



For this project:



- React state is sufficient for local UI state.
- A `useGame()` hook should coordinate game-session behavior.
- The latest backend response should be treated as authoritative game state.



Avoid Redux, Zustand, MobX, or other global state-management libraries unless the application's requirements materially change.



Avoid duplicate sources of truth.



For example, prefer:



```ts
const isGameOver = game.status !== 'InProgress';
```



over storing `isGameOver` as another independent state variable.



## TypeScript



Use strict TypeScript.



Prefer explicit domain-oriented types such as:



```ts
type GameStatus = 'InProgress' | 'Won' | 'Draw';

type GameMode = 'TwoPlayer' | 'Computer';

type PlayerSymbol = 'X' | 'O';
```



Use typed models for:



- game state



- scoreboard state



- moves



- positions



- API requests



- API responses



Avoid `any`.



Use `unknown` for genuinely untyped external data and narrow it before use.



Frontend types should align with the backend API contract rather than independently redefining backend business behavior.



## Routing



Use React Router.



Required routes:



```text
/
/game/:gameId
```



The Game page must read `gameId` from the route and load the corresponding game:



```text
/game/:gameId
      ↓
GET /api/games/:gameId
      ↓
render authoritative state
```



If a game cannot be loaded:



- show a clear error state



- provide a way back to `/`



- do not render fabricated game data



## Tailwind CSS



Use Tailwind CSS for normal component styling.



Prefer Tailwind utilities and configured theme tokens over creating a separate CSS file for every component.



Use reusable React components when a repeated Tailwind pattern becomes complex rather than allowing huge duplicated class strings.



A small global stylesheet is acceptable for:



- base styles



- global CSS variables if required



- special keyframe definitions



- browser normalization



Do not default to ordinary component-level CSS files.



## Design Tokens



Use semantic design tokens instead of scattering raw color values through components.



Prefer names such as:



```text
background
surface
surface-raised
surface-elevated
border
text-primary
text-secondary
text-muted
primary
secondary
success
warning
danger
```



Avoid component-specific or ambiguous names such as:



```text
purple-button
gray-box
dark-card
```



The approved current design tokens are:



```text
Background:       #0B0D12
Surface:          #131722
Raised surface:   #191E2A
Elevated surface: #212838
Border:            #282E3A

Text primary:     #F5F7FA
Text secondary:   #9AA3B2
Text muted:        #64748B

Primary / X:      #7C5CFC
Secondary:        #A78BFA
Success / O:      #34D399
Warning / Draw:   #FBBF24
Error:            #F87171
```



Map these values into the Tailwind theme so components can use semantic utilities such as:



```text
bg-background
bg-surface
bg-surface-raised
text-text-primary
text-text-secondary
border-border
bg-primary
bg-success
bg-warning
bg-danger
```



Avoid unnecessary arbitrary Tailwind values such as `bg-[#7C5CFC]` when a semantic token exists.



## Visual Design Source



The `wireframes/` directory is the approved visual reference for the frontend.



Use it for:



- page layout



- spacing



- typography



- colors



- board geometry



- component hierarchy



- interaction states



- game-over presentation



A generated wireframe must not introduce requirements that are absent from the assignment.



If a design mockup contains unsupported features, do not implement those features merely because they appear visually.



## Home Page Guidelines



The `/` route should be a focused game-mode selection screen.



It should provide:



- Tic Tac Toe product identity



- Two Player / Multiplayer option



- Vs Computer option



- Start Game action



When a mode is selected:



```text
POST /api/games
      ↓
receive gameId
      ↓
navigate(`/game/${gameId}`)
```



Do not add a dashboard, leaderboard, previous-game list, settings, or account system.



## Game Page Guidelines



The `/game/:gameId` route should contain the complete active game experience.



It should display:



- game mode



- current player / turn



- 3 × 3 board



- current-game move activity



- session scoreboard



- Undo Last Move



- Reset Game



- computer-thinking state where applicable



The board should remain the visual focal point.



## Board Guidelines



Each cell must be implemented as an accessible interactive button.



Empty cell:



- interactive while the game is in progress



- keyboard accessible



- clear hover state



- clear focus state



Occupied cell:



- locked



- displays the authoritative X or O



Winning cell:



- uses backend-provided `winningCells`



- has a clear victory highlight



Completed game:



- board is non-interactive



Computer thinking:



- board is temporarily non-interactive



Do not calculate wins or winning cells on the frontend.



## Move Activity Guidelines



The side activity panel is the presentation of the current game's move history.



Each item should display:



- move number



- player



- row



- column



It may be styled as an activity feed or move ledger, but it must represent the backend's current-game move history.



Do not create a second client-side history mechanism.



Do not turn this into an archive of previous games.



## Scoreboard Guidelines



The scoreboard is session-level.



Display:



- X wins



- Draws



- O wins



The values must come from the backend.



Do not calculate scoreboard totals in React from move history.



Do not turn the scoreboard into a leaderboard.



## Undo Guidelines



The frontend should reflect the authoritative availability of Undo.



Undo is:



- available while the game is in progress and there is an undoable move



- disabled when there are no moves to undo



- disabled after `Won`



- disabled after `Draw`



The frontend must call the backend to perform Undo.



After a successful Undo, replace the current React game state with the backend response.



Do not manually mutate the board or move history to simulate Undo.



## Game-Over Modal Guidelines



When backend state becomes `Won` or `Draw`:



- lock the board



- disable Undo



- highlight winning cells when applicable



- display the result modal



The modal should contain:



### Win



- winner



- concise result message



- `Replay`



- `Export Move Log`



- `Go Home`



### Draw



- draw result



- concise result message



- `Replay`



- `Export Move Log`



- `Go Home`



`Replay` resets the current game and keeps the user on `/game/:gameId`.



`Go Home` navigates to `/`.



Do not create a separate game-over route.



## Loading and Error States



Every asynchronous screen or action should have an intentional loading and error state.



Examples:



```text
Loading game...
```



```text
Unable to connect to the game server.

[ TRY AGAIN ] [ GO HOME ]
```



```text
Game session not found.

[ GO HOME ]
```



Do not display fake board state before the real backend response is available.



Do not expose raw stack traces or implementation details to users.



## Accessibility



Use semantic HTML and keyboard-accessible controls.



Requirements include:



- board cells are `<button>` elements



- controls are actual buttons



- visible keyboard focus states



- accessible labels for board cells



- disabled states are visually and semantically clear



- color is not the only indicator of game state



- modal actions are keyboard accessible



Example board-cell labels:



```text
Row 1, Column 2, empty cell
Row 1, Column 2, occupied by X
```



Prefer accessibility-first selectors such as `getByRole` and `getByLabelText` in frontend tests.



## Responsive Design



Desktop/laptop browser is the primary target.



Desktop layout should preserve:



```text
Player/session information | Board | Scoreboard/activity
```



Smaller screens should stack content without horizontal overflow:



```text
Status
Board
Controls
Scoreboard
Activity
```



The board must remain square and visually prominent.



Use Tailwind responsive utilities rather than separate duplicated mobile implementations.



## Interaction and Animation



Use subtle, short interactions for:



- mode selection



- cell hover



- cell focus



- X/O appearance



- button hover



- active turn indication



- winning-cell emphasis



- modal entrance



Prefer CSS/Tailwind transitions for simple interactions.



Avoid:



- confetti



- particle effects



- excessive neon effects



- long blocking animations



- animations that delay gameplay



Respect `prefers-reduced-motion` for non-essential motion.



## Icons



Use Lucide React or the existing approved icon library.



Do not use emoji as primary interface iconography.



Maintain consistent:



- stroke weight



- icon size



- alignment



Icons should support labels rather than replace important text.



## Performance



This application is small. Prioritize simplicity and maintainability over premature optimization.



Do:



- minimize dependencies



- avoid unnecessary global state



- avoid repeated expensive computations



- use stable keys for lists



- keep API responses authoritative



Do not add complex caching, virtualization, or widespread memoization without a concrete need.



## Testing



Frontend tests should validate UI behavior and API integration boundaries.



Good candidates include:



- Home renders both game modes.



- Starting a mode creates a game and navigates to `/game/:gameId`.



- Game page loads the game using the route ID.



- Authoritative board state renders correctly.



- Empty cells are interactive only when appropriate.



- Occupied cells are disabled.



- Undo state is correct.



- Win/draw modal renders from backend state.



- Replay invokes reset.



- Go Home navigates to `/`.



- API errors render the correct error state.



Do not duplicate the backend's complete game-rule test suite in React.



## Naming Conventions



Components:



```text
GameBoard.tsx
GameOverModal.tsx
Scoreboard.tsx
```



Hooks:



```text
useGame.ts
```



Services:



```text
gameApi.ts
```



Types:



```text
GameState
GameStatus
Move
Position
```



Use domain-oriented names rather than names based only on visual styling.



## AI-Assisted Frontend Development



AI may be used to generate frontend code.



Before accepting generated code:



1. Check the assignment requirements.



2. Check `AGENTS.md`.



3. Check the relevant files under `docs/`.



4. Check the approved designs under `wireframes/`.



5. Check the existing frontend implementation and tests.



6. Prefer a small, verifiable change.



Review generated code for:



- invented product features



- duplicate backend business logic



- unnecessary dependencies



- accessibility problems



- inconsistent design tokens



- responsive issues



- excessive abstraction



- API-contract mismatches



The developer should be able to explain why each generated component exists and how it maps to the product requirements or approved design.



## Frontend Completion Standard



A frontend feature is complete when:



1. It maps to a known requirement or approved design decision.



2. It behaves correctly against the real backend.



3. It uses the established design tokens.



4. Loading, error, disabled, and completed states have been considered.



5. It is accessible.



6. It does not duplicate authoritative backend logic.



7. Appropriate tests exist.



8. No unnecessary dependencies or abstractions were introduced.



9. Relevant documentation is updated.



10. The implementation is visually consistent with the approved wireframes.



