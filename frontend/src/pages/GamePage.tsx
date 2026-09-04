import { Hash } from 'lucide-react'
import { useNavigate, useParams } from 'react-router-dom'
import { GameBoard } from '../components/game/GameBoard'
import GameControls from '../components/game/GameControls'
import GameOverModal from '../components/game/GameOverModal'
import GameStatus from '../components/game/GameStatus'
import { MoveActivity, Scoreboard } from '../components/game/MoveActivity'
import PlayerPanel from '../components/game/PlayerPanel'
import Button from '../components/ui/Button'
import Spinner from '../components/ui/Spinner'
import { useGame } from '../hooks/useGame'

/**
 * GamePage — The active game session screen.
 *
 * Layout (from wireframes/GamePage_Multiplayer.png):
 *   Header
 *   ┌─ Left col ──┬─ Center col ──────────────┬─ Right col ─┐
 *   │ PlayerPanel │ GameStatus                 │ Scoreboard  │
 *   │             │ GameBoard                  │             │
 *   │             │ GameControls               │ MoveActivity│
 *   └─────────────┴────────────────────────────┴─────────────┘
 *
 * On mobile the columns stack vertically.
 *
 * Error / loading states (from frontend-guidelines §24–25):
 *   Loading → spinner
 *   Not found → "Game session not found" + Go Home
 *   Error → "Unable to connect…" + Try Again + Go Home
 *
 * GameOverModal is rendered when game.status !== 'InProgress'.
 */
export default function GamePage() {
  const { gameId } = useParams<{ gameId: string }>()
  const navigate = useNavigate()

  if (!gameId) {
    navigate('/')
    return null
  }

  const {
    game,
    scoreboard,
    isLoading,
    isSubmitting,
    isComputerThinking,
    error,
    actionError,
    isNotFound,
    isGameOver,
    canUndo,
    makeMove,
    undoMove,
    resetGame,
    resetScoreboard,
    retryLoad,
  } = useGame(gameId)

  // ---- Loading state ----
  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-4">
        <Spinner size={32} label="Loading game…" />
        <p className="text-text-secondary text-sm font-mono-tabular">Loading game…</p>
      </div>
    )
  }

  // ---- Not found ----
  if (isNotFound) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-6 px-4">
        <div className="text-center">
          <p className="font-display font-bold text-xl text-text-primary mb-2">Game session not found.</p>
          <p className="text-text-secondary text-sm">The game with ID <span className="font-mono-tabular text-text-muted">{gameId}</span> does not exist.</p>
        </div>
        <Button id="not-found-go-home-btn" variant="primary" onClick={() => navigate('/')}>
          Go Home
        </Button>
      </div>
    )
  }

  // ---- Server error ----
  if (error || !game) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-6 px-4">
        <div className="text-center">
          <p className="font-display font-bold text-xl text-text-primary mb-2">Unable to connect to the game server.</p>
          <p className="text-text-secondary text-sm">{error ?? 'Please check that the backend is running.'}</p>
        </div>
        <div className="flex gap-3">
          <Button id="error-retry-btn" variant="secondary" onClick={retryLoad}>
            Try Again
          </Button>
          <Button id="error-go-home-btn" variant="primary" onClick={() => navigate('/')}>
            Go Home
          </Button>
        </div>
      </div>
    )
  }

  const shortId = game.gameId.slice(0, 8).toUpperCase()
  const isComputerMode = game.gameType === 'Computer'

  return (
    <div className="min-h-screen flex flex-col">
      {/* ---- Header ---- */}
      <header className="flex items-center justify-between gap-4 px-6 py-4 border-b border-border">
        {/* Left — brand */}
        <button
          id="header-home-btn"
          onClick={() => navigate('/')}
          className="flex items-center gap-3 hover:opacity-80 transition-opacity focus-visible:ring-2 focus-visible:ring-primary rounded-lg outline-none"
          aria-label="Go to home page"
        >
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-text-primary font-display font-bold text-sm select-none">
            <Hash size={14} />
          </div>
          <div className="text-left hidden sm:block">
            <span className="font-display font-bold text-text-primary tracking-tight text-sm">TIC TAC TOE</span>
            <p className="text-text-muted text-xs font-mono-tabular">React + .NET Engine</p>
          </div>
        </button>

        {/* Center — mode badge */}
        <div className="flex items-center gap-3">
          <span className="font-mono-tabular text-xs text-text-muted uppercase tracking-widest">
            {isComputerMode ? '▣ VS COMPUTER' : '⊞ MULTIPLAYER'}
          </span>
          <span className="rounded-full border border-border bg-surface-raised px-3 py-1 font-mono-tabular text-xs text-text-muted">
            #{shortId}
          </span>
        </div>

        {/* Right — status indicator */}
        <div className="flex items-center gap-2">
          <span className={`inline-block h-2 w-2 rounded-full ${game.status === 'InProgress' ? 'bg-success pulse-dot' : 'bg-text-muted'}`} aria-hidden="true" />
          <span className="font-mono-tabular text-xs text-text-muted hidden sm:inline">
            {game.status === 'InProgress' ? 'Match Active' : 'Match Ended'}
          </span>
        </div>
      </header>

      {/* ---- Action error (transient) ---- */}
      {actionError && (
        <div
          role="alert"
          className="mx-4 mt-3 rounded-lg border border-danger/30 bg-danger/10 px-4 py-2.5 text-sm text-danger"
        >
          {actionError}
        </div>
      )}

      {/* ---- Main layout ---- */}
      <main className="flex flex-1 gap-5 p-5 xl:p-8 max-w-[1200px] mx-auto w-full">
        {/* Left column — Players */}
        <aside className="hidden lg:flex flex-col gap-4 w-72 shrink-0">
          <PlayerPanel game={game} />
        </aside>

        {/* Center column — Board + Status + Controls */}
        <section className="flex flex-1 flex-col gap-4 min-w-0">
          {/* Mobile: player panel above board */}
          <div className="lg:hidden">
            <PlayerPanel game={game} />
          </div>

          <GameStatus
            game={game}
            isComputerThinking={isComputerThinking}
            moveCount={game.moves.length}
          />

          <GameBoard
            game={game}
            isSubmitting={isSubmitting}
            isComputerThinking={isComputerThinking}
            onCellClick={(row, col) => makeMove(game.currentPlayer, row, col)}
          />

          <GameControls
            canUndo={canUndo}
            isSubmitting={isSubmitting}
            onUndo={undoMove}
            onReset={resetGame}
          />
        </section>

        {/* Right column — Scoreboard + Activity */}
        <aside className="hidden lg:flex flex-col gap-4 w-72 shrink-0">
          {scoreboard && (
            <Scoreboard
              scoreboard={scoreboard}
              isResetting={isSubmitting}
              onReset={resetScoreboard}
            />
          )}
          <MoveActivity moves={game.moves} />
        </aside>

        {/* Mobile: scoreboard + activity below board */}
        <div className="flex flex-col gap-4 lg:hidden w-full">
          {scoreboard && (
            <Scoreboard
              scoreboard={scoreboard}
              isResetting={isSubmitting}
              onReset={resetScoreboard}
            />
          )}
          <MoveActivity moves={game.moves} />
        </div>
      </main>

      {/* ---- Game Over Modal ---- */}
      {isGameOver && (
        <GameOverModal
          game={game}
          onReplay={resetGame}
          isResetting={isSubmitting}
        />
      )}
    </div>
  )
}
