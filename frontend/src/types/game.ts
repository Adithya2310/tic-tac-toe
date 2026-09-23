/**
 * Domain types for the Tic Tac Toe frontend.
 *
 * These types are aligned with the .NET REST API contract in docs/backend/api-contract.md.
 * Property names exactly match the JSON keys returned by the backend (camelCase).
 *
 * The frontend never computes authoritative game state — it renders whatever
 * the backend returns. These types are purely for structuring that returned data.
 */

// ------------------------------------------------------------------
// Primitive domain types
// ------------------------------------------------------------------

import { GameMode, GameStatus, PlayerSymbol } from '../constants/game'

/** The symbol a player uses. Empty string means the cell is unoccupied. */
export type CellValue = PlayerSymbol | '';

/** The two player symbols. */
export type PlayerSymbol = typeof PlayerSymbol[keyof typeof PlayerSymbol];

/** Game mode as returned by the backend. */
export type GameMode = typeof GameMode[keyof typeof GameMode];

/** Authoritative game status. The frontend never computes this. */
export type GameStatus = typeof GameStatus[keyof typeof GameStatus];

// ------------------------------------------------------------------
// API response shapes (mirror api-contract.md exactly)
// ------------------------------------------------------------------

/** A single board cell coordinate. */
export interface Position {
  row: number;
  column: number;
}

/** A single move in the game history. */
export interface Move {
  moveNumber: number;
  player: PlayerSymbol;
  row: number;
  column: number;
}

/**
 * The authoritative game state returned by all game endpoints.
 * The frontend renders this directly — it never recomputes winner,
 * winningCells, currentPlayer, or status independently.
 */
export interface GameState {
  gameId: string;
  /** Side length of the board (n for an n×n grid). */
  boardSize: number;
  /** n×n board: board[row][col] is 'X', 'O', or ''. */
  board: CellValue[][];
  currentPlayer: PlayerSymbol;
  gameType: GameMode;
  status: GameStatus;
  winner: PlayerSymbol | null;
  winningCells: Position[];
  moves: Move[];
}

/** Session-level scoreboard returned by GET /api/scoreboard. */
export interface ScoreboardState {
  xWins: number;
  oWins: number;
  draws: number;
}

// ------------------------------------------------------------------
// Request shapes (sent to the API)
// ------------------------------------------------------------------

export interface CreateGameRequest {
  gameType: GameMode;
  /** Side length of the board. Defaults to 3 on the backend when omitted. */
  boardSize?: number;
}

export interface MakeMoveRequest {
  player: PlayerSymbol;
  row: number;
  column: number;
}

// ------------------------------------------------------------------
// API error shape (from api-contract.md §10)
// ------------------------------------------------------------------

export interface ApiError {
  code: string;
  message: string;
}

// ------------------------------------------------------------------
// UI-only state helpers (not from the API)
// ------------------------------------------------------------------

/** Discriminated union for the async load state of a game page. */
export type GameLoadState =
  | { kind: 'idle' }
  | { kind: 'loading' }
  | { kind: 'loaded'; game: GameState }
  | { kind: 'not-found' }
  | { kind: 'error'; message: string };
