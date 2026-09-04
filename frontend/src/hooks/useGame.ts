import { useCallback, useEffect, useReducer, useRef } from 'react'
import * as gameApi from '../services/gameApi'
import type { GameState, PlayerSymbol, ScoreboardState } from '../types/game'

// ------------------------------------------------------------------
// State shape
// ------------------------------------------------------------------

interface UseGameState {
  /** null while loading or on error */
  game: GameState | null
  scoreboard: ScoreboardState | null
  /** Initial game load */
  isLoading: boolean
  /** A move / undo / reset is in flight */
  isSubmitting: boolean
  /** Computer is moving (subset of isSubmitting, shown on board) */
  isComputerThinking: boolean
  /** Persistent error (game not found / server down) */
  error: string | null
  /** Transient action error (bad move etc.) — clears on next action */
  actionError: string | null
  /** gameId not found in the backend */
  isNotFound: boolean
}

type Action =
  | { type: 'LOAD_START' }
  | { type: 'LOAD_SUCCESS'; game: GameState; scoreboard: ScoreboardState }
  | { type: 'LOAD_NOT_FOUND' }
  | { type: 'LOAD_ERROR'; message: string }
  | { type: 'SUBMIT_START'; computerThinking?: boolean }
  | { type: 'SUBMIT_SUCCESS'; game: GameState; scoreboard: ScoreboardState }
  | { type: 'SUBMIT_ERROR'; message: string }
  | { type: 'CLEAR_ACTION_ERROR' }

function reducer(state: UseGameState, action: Action): UseGameState {
  switch (action.type) {
    case 'LOAD_START':
      return { ...state, isLoading: true, error: null, isNotFound: false }
    case 'LOAD_SUCCESS':
      return { ...state, isLoading: false, game: action.game, scoreboard: action.scoreboard }
    case 'LOAD_NOT_FOUND':
      return { ...state, isLoading: false, isNotFound: true }
    case 'LOAD_ERROR':
      return { ...state, isLoading: false, error: action.message }
    case 'SUBMIT_START':
      return { ...state, isSubmitting: true, actionError: null, isComputerThinking: action.computerThinking ?? false }
    case 'SUBMIT_SUCCESS':
      return { ...state, isSubmitting: false, isComputerThinking: false, game: action.game, scoreboard: action.scoreboard }
    case 'SUBMIT_ERROR':
      return { ...state, isSubmitting: false, isComputerThinking: false, actionError: action.message }
    case 'CLEAR_ACTION_ERROR':
      return { ...state, actionError: null }
    default:
      return state
  }
}

const initialState: UseGameState = {
  game: null,
  scoreboard: null,
  isLoading: true,
  isSubmitting: false,
  isComputerThinking: false,
  error: null,
  actionError: null,
  isNotFound: false,
}

// ------------------------------------------------------------------
// Hook
// ------------------------------------------------------------------

/**
 * useGame — central orchestration hook for the game page.
 *
 * All backend interactions flow through this hook.
 * Components receive game state and action functions — they never call
 * gameApi directly.
 */
export function useGame(gameId: string) {
  const [state, dispatch] = useReducer(reducer, initialState)
  // Guard against setting state after unmount
  const mounted = useRef(true)
  useEffect(() => {
    mounted.current = true
    return () => { mounted.current = false }
  }, [])

  // ---- Helpers ----

  /** Fetches the scoreboard and returns it. */
  async function fetchScoreboard(): Promise<ScoreboardState> {
    return gameApi.getScoreboard()
  }

  // ---- Load game ----

  const loadGame = useCallback(async () => {
    if (!mounted.current) return
    dispatch({ type: 'LOAD_START' })
    try {
      const [game, scoreboard] = await Promise.all([
        gameApi.getGame(gameId),
        gameApi.getScoreboard(),
      ])
      if (!mounted.current) return
      dispatch({ type: 'LOAD_SUCCESS', game, scoreboard })
    } catch (err) {
      if (!mounted.current) return
      const code = (err as { code?: string }).code
      if (code === 'GAME_NOT_FOUND') {
        dispatch({ type: 'LOAD_NOT_FOUND' })
      } else {
        dispatch({ type: 'LOAD_ERROR', message: err instanceof Error ? err.message : 'Failed to load game.' })
      }
    }
  }, [gameId])

  useEffect(() => { loadGame() }, [loadGame])

  // ---- Make move ----

  const makeMove = useCallback(async (player: PlayerSymbol, row: number, column: number) => {
    if (state.isSubmitting || !state.game) return
    const isComputerMode = state.game.gameType === 'Computer'
    dispatch({ type: 'SUBMIT_START', computerThinking: isComputerMode })
    try {
      const game = await gameApi.makeMove(gameId, player, row, column)
      const scoreboard = await fetchScoreboard()
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_SUCCESS', game, scoreboard })
    } catch (err) {
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_ERROR', message: err instanceof Error ? err.message : 'Move failed.' })
    }
  }, [gameId, state.isSubmitting, state.game])

  // ---- Undo ----

  const undoMove = useCallback(async () => {
    if (state.isSubmitting || !state.game) return
    dispatch({ type: 'SUBMIT_START' })
    try {
      const game = await gameApi.undo(gameId)
      const scoreboard = await fetchScoreboard()
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_SUCCESS', game, scoreboard })
    } catch (err) {
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_ERROR', message: err instanceof Error ? err.message : 'Undo failed.' })
    }
  }, [gameId, state.isSubmitting, state.game])

  // ---- Reset game ----

  const resetGame = useCallback(async () => {
    if (state.isSubmitting || !state.game) return
    dispatch({ type: 'SUBMIT_START' })
    try {
      const game = await gameApi.resetGame(gameId)
      const scoreboard = await fetchScoreboard()
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_SUCCESS', game, scoreboard })
    } catch (err) {
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_ERROR', message: err instanceof Error ? err.message : 'Reset failed.' })
    }
  }, [gameId, state.isSubmitting, state.game])

  // ---- Reset scoreboard ----

  const resetScoreboard = useCallback(async () => {
    if (state.isSubmitting) return
    dispatch({ type: 'SUBMIT_START' })
    try {
      const scoreboard = await gameApi.resetScoreboard()
      if (!mounted.current) return
      // Keep game state, just update scoreboard
      if (state.game) {
        dispatch({ type: 'SUBMIT_SUCCESS', game: state.game, scoreboard })
      }
    } catch (err) {
      if (!mounted.current) return
      dispatch({ type: 'SUBMIT_ERROR', message: err instanceof Error ? err.message : 'Scoreboard reset failed.' })
    }
  }, [gameId, state.isSubmitting, state.game])

  // ---- Derived values (avoid redundant state) ----

  const isGameOver = state.game != null && state.game.status !== 'InProgress'

  /** Undo is available only while the game is in progress and there is at least one move. */
  const canUndo =
    state.game != null &&
    state.game.status === 'InProgress' &&
    state.game.moves.length > 0 &&
    !state.isSubmitting

  return {
    ...state,
    isGameOver,
    canUndo,
    makeMove,
    undoMove,
    resetGame,
    resetScoreboard,
    retryLoad: loadGame,
    clearActionError: () => dispatch({ type: 'CLEAR_ACTION_ERROR' }),
  }
}
