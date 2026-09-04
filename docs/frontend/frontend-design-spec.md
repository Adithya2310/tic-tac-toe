# Frontend Design Specification

## 1. Purpose

This document defines the React frontend design for the Tic Tac Toe assignment.

The frontend is intentionally lightweight and implements only the product surface required for the assignment:

- Home / game-mode selection
- Game session screen
- Game-over modal states
- Responsive desktop-first presentation
- REST API integration with the .NET backend

The backend remains the source of truth for game state, moves, validation, game status, and scoreboard.

---

## 2. Routes

### `/`

Home page.

Purpose:
- Present the Tic Tac Toe product identity.
- Allow the user to choose a game mode.
- Start a new game through the backend.
- Navigate to the newly created game.

Supported modes:
- Two Player / Multiplayer
- Vs Computer

### `/game/:gameId`

Game page.

Purpose:
- Load the specified game session.
- Render the current board state.
- Show the current turn and game mode.
- Display move activity/history.
- Display the session scoreboard.
- Allow Undo Last Move and Reset Game.
- Show the game-over modal when the backend reports `Won` or `Draw`.

No additional application routes are required.

---

## 3. Page Designs

## 3.1 Home

The home screen is a focused mode-selection experience.

### Layout

- Dark full-page canvas.
- Compact product header.
- Centered hero/title area.
- Large mode-selection panel.
- Two selectable mode cards:
  - Multiplayer / Two Player
  - Vs Computer
- Primary `Start Game` action.
- Small technical/product footer.

### Interaction

1. User selects a mode.
2. React calls `POST /api/games`.
3. Backend returns the new `gameId`.
4. React navigates to `/game/:gameId`.

Do not add:
- Login
- Signup
- User profiles
- Leaderboard
- Previous-games history
- Settings

These are outside the assignment scope.

---

## 3.2 Game

The game screen is the primary application experience.

### Desktop layout

Three functional areas:

- Left: player/session information
- Center: game status + 3x3 board + controls
- Right: scoreboard + move activity

The board remains the visual focal point.

### Main elements

#### Header
- `TIC TAC TOE`
- Optional compact game-session identifier
- Connection/session status may be shown subtly.

Avoid inventing navigation destinations that do not exist in the product.

#### Game status
Show:
- Game mode
- Current player
- Current state

Examples:
- `X's Turn`
- `O's Turn`
- `Computer is thinking...`
- `X Wins!`
- `O Wins!`
- `It's a Draw`

#### Board
- Fixed 3x3 square grid.
- Nine interactive cells.
- Empty cells respond to hover/focus.
- Occupied cells are locked.
- Winning cells receive a success highlight.
- Board interaction is disabled when the game is complete.
- Board interaction is temporarily disabled while the computer is making its move.

#### Game Activity
Present the required current-game move history as a chronological activity feed.

Each entry contains:
- Move number
- Player
- Row and column

Example:

`01  X  Row 1 • Column 1`

`02  O  Row 2 • Column 2`

The latest move may be visually emphasized.

#### Scoreboard
Display the session-level:
- X wins
- Draws
- O wins

Include `Reset Scoreboard`.

Do not turn this into a leaderboard.

#### Controls
- `Undo Last Move`
- `Reset Game`

Undo is:
- Disabled when there are no moves.
- Disabled after game completion, following the selected assignment approach.

Reset Game:
- Clears the current board and move history.
- Starts a fresh game state.
- Does not modify the scoreboard.

---

## 4. Game-Over Modal

A modal overlay is shown when the backend returns a completed game.

### Win

Title:
- `X WINS!` or `O WINS!`

Supporting text:
- `Three in a row.`

Actions:
- `REPLAY`
- `EXPORT MOVE LOG`
- `GO HOME`

### Draw

Title:
- `IT'S A DRAW`

Supporting text:
- `Neither player could claim the board.`

Actions:
- `REPLAY`
- `EXPORT MOVE LOG`
- `GO HOME`

The final board remains visible behind the modal.

### Replay behavior

Replay resets the current game and keeps the user on the same game route.

### Go Home behavior

Navigates to `/` so the user can choose a new mode.



---

## 5. UI States

The Game page must support these states:

1. Fresh game
   - Empty board
   - X's turn
   - Empty activity
   - Undo disabled

2. Game in progress
   - Partially filled board
   - Updated move activity
   - Undo enabled

3. Computer thinking
   - Human is X
   - Computer is O
   - Board temporarily disabled
   - `Computer is thinking...` indicator

4. X wins
   - Winning cells highlighted
   - Board locked
   - Undo disabled
   - Win modal displayed

5. O wins
   - Winning cells highlighted
   - Board locked
   - Undo disabled
   - Win modal displayed

6. Draw
   - Board full
   - Board locked
   - Undo disabled
   - Draw modal displayed

7. Reset
   - Fresh board
   - Activity cleared
   - Scoreboard unchanged

8. API error
   - Clear error message
   - Retry action where appropriate
   - Do not leave the UI in a misleading state

---

## 6. React Component Architecture

Recommended structure:

```text
src/
├── components/
│   ├── Header/
│   ├── GameModeSelector/
│   ├── GameStatus/
│   ├── GameBoard/
│   │   └── BoardCell/
│   ├── PlayerPanel/
│   ├── Scoreboard/
│   ├── MoveActivity/
│   ├── GameControls/
│   └── GameOverModal/
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
├── pages/
│   ├── HomePage.tsx
│   └── GamePage.tsx
│
├── styles/
│   └── globals.css
│
├── App.tsx
└── main.tsx
```

Keep the architecture lightweight.

Do not introduce Redux or another global state library unless a concrete need appears.

---

## 7. Frontend State Model

React should primarily hold the latest backend state and UI-only state.

Suggested game model:

```ts
type GameMode = 'TwoPlayer' | 'Computer';

type GameStatus = 'InProgress' | 'Won' | 'Draw';

type SymbolValue = 'X' | 'O' | null;

interface Position {
  row: number;
  column: number;
}

interface Move {
  moveNumber: number;
  player: 'X' | 'O';
  row: number;
  column: number;
}

interface GameState {
  gameId: string;
  board: SymbolValue[][];
  currentPlayer: 'X' | 'O';
  mode: GameMode;
  status: GameStatus;
  winner: 'X' | 'O' | null;
  winningCells: Position[];
  moves: Move[];
}
```

The exact property names should match the completed backend API contract.

---

## 8. API Boundary

Create a dedicated `gameApi.ts` module.

Expected operations:

```text
createGame(mode)
getGame(gameId)
makeMove(gameId, player, row, column)
undo(gameId)
resetGame(gameId)
getScoreboard()
resetScoreboard()
```

React components should not call `fetch` directly.

Preferred flow:

```text
Component
   ↓
useGame()
   ↓
gameApi.ts
   ↓
.NET REST API
   ↓
latest GameState
   ↓
React render
```

The frontend should not duplicate backend validation or authoritative game rules.

---

## 9. Visual Design System

The Stitch design uses the following visual direction.

### Surfaces

- Canvas: `#0B0D12`
- Primary surface: `#131722`
- Raised surface: `#191E2A`
- Elevated/overlay surface: `#212838`
- Structural border: `#282E3A`

### Text

- Primary: `#F5F7FA`
- Secondary: `#9AA3B2`
- Muted: `#64748B`

### Semantic accents

- Primary / X: `#7C5CFC`
- Secondary accent: `#A78BFA`
- Success / victory / O: `#34D399`
- Draw / warning: `#FBBF24`
- Error: `#F87171`

### Typography

- Plus Jakarta Sans for major headings.
- Inter for body/UI text.
- JetBrains Mono for technical/session information, move numbers, coordinates and counters.

### Geometry

- Cards: approximately 16px radius.
- Board frame: approximately 24px radius.
- Board cells: approximately 16px radius.
- Status pills: fully rounded.
- Use an 8px spacing system with a 4px micro-grid for compact metadata.

---

## 10. Board Interaction Design

### Empty cell

- Raised dark surface.
- Subtle border.
- Hover border/accent.
- Optional faint preview of the current player's marker.

### Occupied cell

- Locked.
- Strong X/O glyph.
- X uses purple/indigo.
- O uses green.

### Winning cell

- Success/accent border.
- Subtle glow.
- Small elevation increase.
- Clear enough to identify the winning three cells immediately.

Transitions should be short and subtle, around 180ms.

Avoid:
- Confetti
- Particle effects
- Large arcade animations
- Excessive neon glow

---

## 11. Responsive Behavior

### Desktop: >= 1024px

Use the three-area game layout with:
- left utility/player area
- center board/stage
- right scoreboard/activity area

### Tablet: 768px–1023px

Use a two-column arrangement where the board remains dominant and activity/telemetry stacks beside or below it.

### Mobile: < 768px

Use a single-column flow:
- status
- board
- controls
- scoreboard
- activity

The board should remain square and fit comfortably within the viewport.

---

## 12. Accessibility

- Maintain strong text/background contrast.
- Provide visible keyboard focus states.
- Use actual buttons for clickable board cells and controls.
- Disabled states must be visually obvious.
- Do not communicate game state through color alone.
- Keep labels readable at laptop viewing sizes.

---

## 13. Scope Rules

The following Stitch-generated visual concepts are intentionally excluded from the implementation unless explicitly required later:

- Product tiers such as `PRO`
- Arena/API navigation tabs
- Match timer

- Leaderboards
- Historical game archive
- Player accounts
- Rankings
- Advanced analytics
- Unrequested game settings

These may have appeared in generated mockups, but they are not part of the agreed product scope.

The design language can be reused; the feature scope should remain aligned with the assignment.

---

## 14. Design Reference

The Stitch-generated screenshots are stored alongside this document as visual references.

They should be treated as the visual target for:
- layout
- hierarchy
- spacing
- color
- typography
- component styling
- game-state presentation

The React implementation should reproduce the design intent rather than copy unnecessary generated features.

---

## 15. Implementation Principle

The frontend goal is:

**Small application + clean architecture + excellent visual polish.**

The implementation should remain easy to explain during the interview and should prioritize:
- correctness
- backend integration
- clear state rendering
- accessibility
- visual consistency
- maintainability
