/**
 * Values defined by the API contract that the UI needs to compare or send.
 *
 * Keep display copy in the component that owns it. These constants are for
 * protocol/domain values, not for one-off labels, Tailwind classes, or prose.
 */
export const PlayerSymbol = {
  X: 'X',
  O: 'O',
} as const

export const GameMode = {
  TwoPlayer: 'TwoPlayer',
  Computer: 'Computer',
} as const

export const GameStatus = {
  InProgress: 'InProgress',
  Won: 'Won',
  Draw: 'Draw',
} as const

export const ApiErrorCode = {
  GameNotFound: 'GAME_NOT_FOUND',
} as const

export const ApiPath = {
  root: '/api',
  games: '/games',
  scoreboard: '/scoreboard',
  scoreboardReset: '/scoreboard/reset',
} as const

export const AppRoute = {
  home: '/',
  game: '/game/:gameId',
  gameSession: (gameId: string) => `/game/${gameId}`,
} as const
