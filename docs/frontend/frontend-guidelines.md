# Frontend Development Guidelines

## 1. Purpose

This document defines the implementation standards for the React + TypeScript frontend of the Tic Tac Toe application.

The frontend should remain lightweight, maintainable, accessible, and visually consistent with the approved designs in `wireframes/`.

These guidelines complement:

- `docs/requirements-traceability.md`
- `docs/architecture.md`
- `docs/domain-model.md`
- `docs/api-contract.md`
- `docs/development-guidelines.md`
- `wireframes/frontend-design-spec.md`
- `AGENTS.md`

The assignment requirements always take precedence over implementation preferences.

---

## 2. Frontend Technology

Use:

- React
- TypeScript
- Vite
- React Router
- Tailwind CSS
- Lucide React for icons where an icon is required
- Native browser APIs / `fetch` unless an HTTP client is already established in the repository

Avoid adding libraries unless they solve a real requirement.

Do not add a large UI component framework solely for convenience.

Do not add Redux, Zustand, MobX, or another global state library for this application unless the state requirements materially change.

---

## 3. Core Frontend Principles

Follow these principles throughout the implementation:

1. Prefer simple solutions over abstractions that are not needed.
2. Keep components focused on presentation and user interaction.
3. Keep API communication outside presentational components.
4. Keep authoritative game rules in the backend.
5. Avoid duplicating backend business logic in React.
6. Prefer composition over deeply nested inheritance-like abstractions.
7. Reuse visual primitives rather than copying styling between components.
8. Make UI states explicit rather than relying on implicit behavior.
9. Optimize for readability and maintainability before micro-optimizations.
10. Every user-visible behavior should have a clear state and error path.

---

## 4. Project Structure

Recommended structure:

```text
src/
├── components/
│   ├── ui/
│   │   ├── Button.tsx
│   │   ├── Card.tsx
│   │   ├── Badge.tsx
│   │   └── ...
│   ├── game/
│   │   ├── GameBoard.tsx
│   │   ├── BoardCell.tsx
│   │   ├── GameStatus.tsx
│   │   ├── GameControls.tsx
│   │   ├── MoveActivity.tsx
│   │   ├── Scoreboard.tsx
│   │   └── GameOverModal.tsx
│   └── home/
│       ├── GameModeSelector.tsx
│       └── ...
│
├── pages/
│   ├── HomePage.tsx
│   └── GamePage.tsx
│
├── hooks/
│   └── useGame.ts
│
├── services/
│   └── gameApi.ts
│
├── types/
│   └── game.ts
│
├── lib/
│   ├── api.ts
│   └── utils.ts
│
├── styles/
│   └── index.css
│
├── App.tsx
└── main.tsx
```

Do not create folders merely to satisfy a pattern. Use the structure as guidance and keep it proportional to the application's size.

---

## 5. TypeScript Standards

Use TypeScript strictly.

Prefer:

```ts
type GameStatus = 'InProgress' | 'Won' | 'Draw';
```

over unbounded strings.

Prefer explicit domain models:

```ts
interface GameState {
  gameId: string;
  board: CellValue[][];
  currentPlayer: PlayerSymbol;
  mode: GameMode;
  status: GameStatus;
  winner: PlayerSymbol | null;
  winningCells: Position[];
  moves: Move[];
}
```

Avoid:

```ts
const game: any = ...
```

Do not use `any` unless there is a documented interoperability reason.

Prefer `unknown` when data is genuinely untyped and validate/narrow it before use.

Use discriminated unions when UI states have materially different data requirements.

Keep frontend types aligned with `docs/api-contract.md`.

---

## 6. Backend Is the Source of Truth

The frontend must not become a second game engine.

Do not implement authoritative:

- move validation
- win detection
- draw detection
- turn switching
- winning-cell calculation
- scoreboard calculation
- computer move selection

The frontend sends user intent to the API and renders the authoritative result.

Preferred flow:

```text
User interaction
      ↓
React event handler
      ↓
useGame()
      ↓
gameApi.ts
      ↓
.NET API
      ↓
authoritative response
      ↓
React state
      ↓
UI
```

UI-only behavior such as opening a modal, showing a spinner, or tracking a temporary request state may remain local to React.

---

## 7. API Layer

Do not call `fetch()` directly from multiple components.

Use a single API boundary, for example:

```text
services/gameApi.ts
```

Example responsibilities:

```ts
createGame(mode)
getGame(gameId)
makeMove(gameId, row, column)
undo(gameId)
resetGame(gameId)
getScoreboard()
resetScoreboard()
```

Components should not know:

- URL construction details
- request headers
- response parsing details
- API error normalization

Those concerns belong in the API/service layer.

### API error handling

Normalize failures into a predictable frontend error shape.

At minimum distinguish:

- network/server unavailable
- HTTP validation/business error
- unexpected server error

Do not expose raw exception messages to users unless they are intentionally safe and user-facing.

---

## 8. State Management

Use the smallest state-management solution that solves the problem.

For this application:

- React component state for local UI state.
- A `useGame` hook for game-session orchestration.
- Server state stored from the latest backend response.

Typical local UI state:

```text
isLoading
isSubmittingMove
isResetting
errorMessage
isGameOverModalOpen
```

Authoritative state:

```text
game
scoreboard
```

Avoid storing the same authoritative value in multiple places.

For example, do not maintain:

```text
game.status
winner
winningCells
```

as independent React states when all three already come from the backend response.

This avoids synchronization bugs.

---

## 9. Async Request Rules

Every asynchronous API action should account for:

1. Loading state.
2. Success state.
3. Error state.
4. Preventing duplicate actions where appropriate.

Example:

```text
Click cell
   ↓
Disable affected interaction
   ↓
Show request state where useful
   ↓
POST move
   ↓
Receive GameState
   ↓
Render new state
```

Avoid optimistic updates for authoritative game state unless there is a concrete need. The game is small and correctness is more important than shaving milliseconds from UI feedback.

For Computer Mode, render a temporary `Computer is thinking...` state while the frontend is waiting for the backend-completed result.

---

## 10. Routing

Use React Router.

Routes:

```text
/
└── HomePage

/game/:gameId
└── GamePage
```

The game page must read `gameId` from the URL.

Recommended behavior:

```text
/game/:gameId
      ↓
GET /api/games/:gameId
      ↓
Render game
```

If the ID is invalid or the game cannot be loaded:

- show a clear error state
- provide a route back to Home
- do not render a fake game

Do not create routes for features that are not in scope.

---

## 11. Design Tokens and Semantic Naming

Do not scatter raw visual values throughout JSX.

Prefer semantic design tokens.

Examples:

```text
--color-background
--color-surface
--color-surface-raised
--color-border
--color-text-primary
--color-text-secondary
--color-accent
--color-success
--color-warning
--color-danger
```

Semantic naming is preferred over purpose-ambiguous names such as:

```text
--purple-500
--gray-900
```

The semantic token should describe what the color means within the interface.

For example:

```text
primary
secondary
success
warning
danger
surface
border
text-primary
text-secondary
```

The actual values may change without requiring component rewrites.

### Approved visual tokens

Current visual design:

```text
Background:        #0B0D12
Surface:           #131722
Raised surface:    #191E2A
Elevated surface:  #212838
Border:            #282E3A

Text primary:      #F5F7FA
Text secondary:    #9AA3B2
Text muted:        #64748B

Primary / X:       #7C5CFC
Secondary accent:  #A78BFA
Success / O:       #34D399
Warning / Draw:    #FBBF24
Error:             #F87171
```

These values are defined by the approved visual design in `wireframes/`.

---

## 12. Tailwind CSS Standards

Use Tailwind CSS for component styling.

Prefer:

```tsx
<button
  className="rounded-xl border border-border bg-surface px-4 py-3 text-sm font-semibold transition hover:bg-surface-raised focus:outline-none focus:ring-2 focus:ring-primary"
>
  Reset Game
</button>
```

over creating a new CSS file for every component.

### Tailwind principles

1. Prefer semantic utility groupings.
2. Reuse common patterns through small reusable React components.
3. Avoid giant unreadable class strings when a component abstraction is clearer.
4. Use the Tailwind theme for project design tokens.
5. Keep arbitrary values to a minimum.
6. Do not bypass the design system by randomly introducing new colors.
7. Use responsive utilities rather than manually writing media queries for ordinary layout behavior.

### When CSS is still appropriate

A small global stylesheet is acceptable for:

- CSS variables/token definitions
- global base styles
- special keyframe animations
- browser-level normalization
- styles that are genuinely awkward or less maintainable in utilities

Do not default to normal CSS for ordinary component layout.

---

## 13. Tailwind Theme Configuration

Map the visual design system into semantic Tailwind tokens.

Conceptually:

```text
bg-background
bg-surface
bg-surface-raised
bg-surface-elevated

text-text-primary
text-text-secondary
text-text-muted

border-border

bg-primary
text-primary
bg-success
bg-warning
bg-danger
```

Avoid making components depend on raw palette values such as:

```text
bg-[#7C5CFC]
```

when the value can be represented by a configured semantic token.

This makes future theme changes much easier.

---

## 14. Component Design

Build components around responsibilities, not around arbitrary DOM fragments.

Good:

```text
GameBoard
BoardCell
Scoreboard
MoveActivity
GameOverModal
```

Avoid:

```text
PurpleBox
LeftPanelThing
BigButtonWrapper
```

A component should generally have one clear reason to change.

Keep pages responsible for composition.

Keep reusable components responsible for reusable UI behavior.

Avoid components that contain unrelated API, routing, game-rule, and styling logic all at once.

---

## 15. UI Components and Variants

For repeated controls, create a consistent visual API.

Example:

```tsx
<Button variant="primary" />
<Button variant="secondary" />
<Button variant="ghost" />
<Button variant="danger" />
```

Prefer a small, intentional variant system over dozens of one-off button styles.

Useful reusable primitives may include:

- Button
- Card
- Badge
- IconButton
- Modal
- Spinner
- StatusPill

Do not build a complete UI framework for this assignment.

Only extract components when reuse or clarity justifies them.

---

## 16. Accessibility

Accessibility is required even for a small game.

### Board

Each cell should be an actual `<button>`.

Provide an accessible label such as:

```text
Row 1, Column 2, empty cell
Row 1, Column 2, occupied by X
```

Do not make clickable `<div>` elements when a button is appropriate.

### Keyboard support

Users should be able to:

- tab through controls
- focus board cells
- activate cells using keyboard input
- operate modal actions using keyboard

### Focus

Do not remove focus outlines without replacing them with a clear accessible focus style.

### Modal

When the game-over modal opens:

- move focus into the modal
- make the modal dismiss/close behavior intentional
- return focus appropriately when practical

### Color

Never rely only on color to communicate:

- current player
- winner
- disabled state
- error state

Combine color with text, icons, labels, or shape.

---

## 17. Board Interaction

The board is the primary interactive component.

Each cell should derive its appearance from:

```text
value
isWinningCell
isInteractive
currentPlayer
isSubmittingMove
```

Do not calculate winning cells on the frontend.

For an empty cell:

- show interactive hover state
- show focus state
- allow click only when the game is in progress

For an occupied cell:

- disable interaction
- show the authoritative X/O value

For a completed game:

- disable all cells

For Computer Mode while waiting for the backend:

- disable the board
- show `Computer is thinking...`

---

## 18. Game Activity / Move History

Treat the activity feed as a presentation of the current game's move history.

Each item should show:

- move number
- player
- row
- column

Do not create a second client-side history system.

Render the list from the backend's `moves` collection.

If the list becomes long, constrain the panel and allow scrolling without expanding the entire page indefinitely.

---

## 19. Scoreboard

The scoreboard is session-level.

Render:

- X wins
- Draws
- O wins

The values must come from backend state.

Do not calculate scoreboard values from the current move list.

Do not transform the scoreboard into a leaderboard.

Do not persist client-only scoreboard values across browser reloads unless the backend provides that state.

---

## 20. Undo

The frontend should determine only whether the button should be interactively available based on authoritative state.

Expected presentation:

```text
No moves / completed game
    ↓
Undo disabled

Game in progress + undoable moves
    ↓
Undo enabled
```

The backend remains responsible for performing the actual undo behavior.

Do not manually mutate the board in React to simulate undo.

After Undo succeeds:

```text
backend response
      ↓
replace current GameState
      ↓
render authoritative board/history/turn
```

---

## 21. Game Over

When `game.status` is:

```text
Won
Draw
```

the UI should:

- lock the board
- disable Undo
- show the final result
- show the winning cells where applicable
- display the game-over modal

Do not use frontend win detection to decide when to show the modal.

The backend response determines completion.

### Modal actions

`Replay`:

```text
reset current game
stay on /game/:gameId
```

`Go Home`:

```text
navigate("/")
```

Do not create a game-over route.

---

## 22. Home Page

Keep the Home page intentionally simple.

Responsibilities:

- product identity
- game-mode selection
- start game

Do not add:

- leaderboard
- previous matches
- account system
- settings
- statistics
- unrelated navigation

Selecting a mode should lead to:

```text
POST /api/games
      ↓
gameId
      ↓
navigate(`/game/${gameId}`)
```

---

## 23. Responsive Design

Desktop is the primary target.

Use responsive Tailwind utilities.

Target behavior:

### Desktop

```text
Players / metadata | Board | Scoreboard / Activity
```

### Smaller screens

Stack the content vertically:

```text
Status
Board
Controls
Scoreboard
Activity
```

The board must remain square and fit within the viewport.

Avoid horizontal scrolling for ordinary laptop/mobile widths.

Do not create a completely separate mobile implementation.

---

## 24. Loading States

Every initial data load should have a visible loading state.

For example:

```text
Loading game...
```

The UI should not briefly display misleading fake game data before the actual game is loaded.

For move submission:

- prevent duplicate submissions
- give the user clear feedback when necessary
- restore interaction after the request completes

For Computer Mode:

```text
Computer is thinking...
```

should remain visible until the authoritative updated game state arrives.

---

## 25. Error States

Errors should be designed, not improvised.

Examples:

### Game not found

```text
Game session not found.

[ GO HOME ]
```

### Server unavailable

```text
Unable to connect to the game server.

Please check that the backend is running.

[ TRY AGAIN ]   [ GO HOME ]
```

### Move rejected

Show a concise user-facing message and refresh/reconcile with authoritative backend state where appropriate.

Never silently swallow an API failure.

Avoid displaying raw stack traces or implementation details.

---

## 26. Notifications

Use transient notifications only for genuinely transient events such as:

- API error
- successful scoreboard reset
- unexpected connection failure

Do not duplicate the required move history as transient toast notifications.

The game activity panel is the persistent current-game record.

---

## 27. Animation and Motion

Use motion sparingly.

Approved interaction categories:

- hover
- focus
- press
- X/O appearance
- active turn transition
- winning-cell emphasis
- modal entrance

Keep durations short and consistent.

Prefer CSS/Tailwind transitions for simple interactions.

Avoid:

- confetti
- particle explosions
- excessive bouncing
- long blocking animations
- animations that delay gameplay

Respect `prefers-reduced-motion` for non-essential motion.

---

## 28. Icons

Use Lucide React or an equivalent existing icon package.

Do not use emoji as the primary iconography.

Keep icon style consistent:

- similar stroke weight
- consistent sizing
- consistent alignment

Icons should support a label rather than replace important text.

---

## 29. Performance

This application is small, so prioritize simplicity.

Do:

- avoid unnecessary dependencies
- avoid unnecessary global state
- keep components reasonably small
- avoid expensive calculations during every render
- use stable list keys
- keep API responses authoritative and minimal

Do not add:

- complex caching systems
- virtualization for tiny lists
- memoization everywhere
- premature code splitting

Use performance optimization only where there is an observable or obvious cost.

---

## 30. Data Fetching and Effects

Use `useEffect` only for genuine side effects.

A typical Game page may:

```text
route gameId changes
      ↓
fetch game
      ↓
set game state
```

Avoid using effects to derive simple values.

Prefer:

```ts
const isGameOver = game.status !== 'InProgress';
```

instead of:

```ts
const [isGameOver, setIsGameOver] = useState(false);
```

Derived state should normally be computed from authoritative state rather than duplicated.

---

## 31. Forms and User Input

The application has minimal form/input requirements.

For any input introduced later:

- use controlled or clearly justified uncontrolled inputs
- validate input
- provide labels
- show useful validation errors
- do not rely only on placeholders

Game board selection should remain button-based rather than form-based.

---

## 32. Security and Reliability

Although this is a local assignment, follow basic secure frontend practices.

Do not:

- store secrets in the frontend
- hard-code backend credentials
- inject unsanitized HTML
- use `dangerouslySetInnerHTML` unnecessarily
- trust URL parameters without handling invalid values
- expose server exception details in the UI

The frontend should assume the backend can reject any request and handle that gracefully.

---

## 33. Testing

Frontend tests should focus on UI behavior and integration boundaries rather than reproducing backend game logic.

Good frontend tests:

- Home renders game-mode options.
- Selecting a mode starts navigation after successful game creation.
- Game page loads a game by route ID.
- Board renders backend state.
- Empty cells are interactive during an active game.
- Occupied cells are disabled.
- Undo is disabled when required.
- Win/draw modal appears for completed backend state.
- Replay invokes reset and keeps the user on the game page.
- Go Home navigates to `/`.
- API errors display appropriate UI.

Do not duplicate full win/draw/computer-rule test suites in React when the backend already owns those rules.

---

## 34. Testing Selectors

Prefer accessible selectors in tests:

```text
getByRole
getByLabelText
getByText
```

Avoid relying primarily on CSS class names or implementation-specific DOM structure.

If a stable selector is genuinely necessary, use a deliberate test identifier rather than coupling tests to styling classes.

---

## 35. Naming Conventions

Use:

### Components

PascalCase:

```text
GameBoard.tsx
GameOverModal.tsx
Scoreboard.tsx
```

### Hooks

camelCase prefixed with `use`:

```text
useGame.ts
```

### Services

camelCase:

```text
gameApi.ts
```

### Types

PascalCase:

```text
GameState
Move
Position
```

Use names that reflect domain concepts rather than visual appearance.

---

## 36. Avoid Prop Drilling by Design, Not by Default

For this small application, a few levels of props are normal.

Do not introduce context solely because a prop travels through two components.

If a state truly becomes cross-cutting and difficult to manage, reassess the architecture before adding a global state library.

---

## 37. Visual Consistency Checklist

Every new component should respect:

- semantic color tokens
- established spacing scale
- established border radius
- established typography
- established focus style
- established button hierarchy
- established card treatment
- established icon sizing

Do not introduce a new shade of purple, gray, green, or border radius without a design reason.

Prefer existing tokens.

---

## 38. Wireframe Fidelity

The `wireframes/` directory is the approved visual reference.

Before implementing a new UI element:

1. Check whether it exists in the wireframes.
2. Check the frontend design specification.
3. Reuse existing component patterns.
4. Add new UI only when required by behavior or usability.

Generated wireframes can contain speculative elements. Do not implement speculative features merely because they appear in a generated design.

The following remain out of scope unless explicitly approved:

- leaderboard
- historical game archive
- analytics dashboard
- match timer
- export move log
- authentication
- profile/settings pages
- API documentation UI
- arbitrary navigation tabs

---

## 39. Code Quality

Prefer code that is obvious to another developer.

Avoid:

- clever one-liners that hide important behavior
- huge components
- deeply nested conditional rendering
- repeated magic values
- repeated API code
- unused abstractions
- dead code
- commented-out alternatives

Comments should explain why something is unusual, not restate what the code obviously does.

---

## 40. Pull Request / Commit Discipline

Keep frontend changes small and reviewable.

A good implementation sequence is:

```text
1. Project bootstrap
2. Tailwind/theme setup
3. Routing
4. Home page
5. Game page static layout
6. Board interaction wiring
7. API integration
8. Loading/error states
9. Game-over states
10. Responsive polish
11. Accessibility review
12. Frontend tests
13. Final cleanup
```

Avoid mixing unrelated refactors into feature commits.

---

## 41. Definition of Done for Frontend Features

A frontend feature is complete when:

- it maps to a known requirement or approved design decision
- it uses the established component/design system
- it is implemented in the correct layer
- loading/error/disabled states are considered
- it is accessible
- it works against the actual backend contract
- it does not duplicate backend business logic
- it has appropriate tests
- it does not introduce unnecessary dependencies
- documentation is updated when behavior or contracts change

---

## 42. Final Frontend Principle

The desired outcome is:

**Lightweight code, strong visual design, explicit state, authoritative backend, and minimal complexity.**

The frontend should feel polished without becoming architecturally heavy.

A developer reviewing the repository should be able to understand the application quickly, trace a user action from React to the REST API, and see that the implementation follows the approved wireframes and project requirements.
