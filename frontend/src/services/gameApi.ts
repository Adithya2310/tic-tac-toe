/**
 * gameApi.ts — Centralised API boundary for all backend communication.
 *
 * Components and hooks never call fetch() directly. All URL construction,
 * request headers, response parsing, and error normalisation live here.
 *
 * The Vite dev-server proxy (vite.config.ts) forwards /api/* to
 * http://localhost:5000, so no hardcoded backend origin is needed.
 */

import type {
  ApiError,
  CreateGameRequest,
  GameState,
  MakeMoveRequest,
  PlayerSymbol,
  ScoreboardState,
} from '../types/game';

// ------------------------------------------------------------------
// Internal helpers
// ------------------------------------------------------------------

const BASE = '/api';

/**
 * Wraps a fetch() call with consistent response handling.
 * On non-2xx responses, it attempts to parse the API error envelope
 * { code, message } and throws a typed ApiError.
 * On network failure, it throws an Error with a user-friendly message.
 */
async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${BASE}${path}`, {
      headers: { 'Content-Type': 'application/json', ...init?.headers },
      ...init,
    });
  } catch {
    throw new Error('Unable to connect to the game server. Please check that the backend is running.');
  }

  if (response.ok) {
    // 204 No Content has no body
    if (response.status === 204) return undefined as T;
    return response.json() as Promise<T>;
  }

  // Attempt to parse the structured API error envelope.
  let apiError: ApiError | null = null;
  try {
    apiError = await response.json();
  } catch {
    // Body was not JSON — fall through to generic error.
  }

  if (apiError?.code) {
    const err = new Error(apiError.message) as Error & { code: string };
    err.code = apiError.code;
    throw err;
  }

  if (response.status === 404) {
    const err = new Error('Game session not found.') as Error & { code: string };
    err.code = 'GAME_NOT_FOUND';
    throw err;
  }

  throw new Error(`Unexpected server error (HTTP ${response.status}).`);
}

// ------------------------------------------------------------------
// Game operations
// ------------------------------------------------------------------

/** POST /api/games — Create a new game and return its initial state. */
export async function createGame(gameType: CreateGameRequest['gameType']): Promise<GameState> {
  return request<GameState>('/games', {
    method: 'POST',
    body: JSON.stringify({ gameType } satisfies CreateGameRequest),
  });
}

/** GET /api/games/:id — Load the current authoritative state of a game. */
export async function getGame(gameId: string): Promise<GameState> {
  return request<GameState>(`/games/${gameId}`);
}

/**
 * POST /api/games/:id/moves — Submit a human player's move.
 * Returns the updated game state (which includes the computer's
 * response move in Computer mode).
 */
export async function makeMove(
  gameId: string,
  player: PlayerSymbol,
  row: number,
  column: number,
): Promise<GameState> {
  return request<GameState>(`/games/${gameId}/moves`, {
    method: 'POST',
    body: JSON.stringify({ player, row, column } satisfies MakeMoveRequest),
  });
}

/** POST /api/games/:id/undo — Undo according to game mode. */
export async function undo(gameId: string): Promise<GameState> {
  return request<GameState>(`/games/${gameId}/undo`, { method: 'POST' });
}

/** POST /api/games/:id/reset — Reset the board (scoreboard unchanged). */
export async function resetGame(gameId: string): Promise<GameState> {
  return request<GameState>(`/games/${gameId}/reset`, { method: 'POST' });
}

// ------------------------------------------------------------------
// Scoreboard operations
// ------------------------------------------------------------------

/** GET /api/scoreboard — Return the session-level scoreboard. */
export async function getScoreboard(): Promise<ScoreboardState> {
  return request<ScoreboardState>('/scoreboard');
}

/** POST /api/scoreboard/reset — Zero out all scoreboard counts. */
export async function resetScoreboard(): Promise<ScoreboardState> {
  return request<ScoreboardState>('/scoreboard/reset', { method: 'POST' });
}
