import { Monitor, Users } from 'lucide-react'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Button from '../components/ui/Button'
import { createGame } from '../services/gameApi'
import type { GameMode } from '../types/game'

/**
 * HomePage — Game-mode selection screen.
 *
 * Responsibilities:
 *   1. Present product identity.
 *   2. Let the user pick a game mode (Two Player / Vs Computer).
 *   3. POST /api/games on Start Game.
 *   4. Navigate to /game/:gameId on success.
 *
 * Out of scope (per design-spec §13 + frontend-guidelines §38):
 *   - PRO badge, version chips, Arena/API nav tabs
 *   - Match timer, leaderboard, player accounts
 */
export default function HomePage() {
  const navigate = useNavigate()
  const [selectedMode, setSelectedMode] = useState<GameMode>('TwoPlayer')
  const [isStarting, setIsStarting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleStartGame() {
    setIsStarting(true)
    setError(null)
    try {
      const game = await createGame(selectedMode)
      navigate(`/game/${game.gameId}`)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to start game. Is the backend running?')
      setIsStarting(false)
    }
  }

  return (
    <div className="min-h-screen flex flex-col">
      {/* ---- Header ---- */}
      <header className="flex items-center gap-3 px-6 py-4 border-b border-border">
        <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-text-primary font-display font-bold text-sm select-none">
          #
        </div>
        <div>
          <span className="font-display font-bold text-text-primary tracking-tight text-sm">TIC TAC TOE</span>
          <p className="text-text-muted text-xs font-mono-tabular">React + .NET Engine</p>
        </div>
      </header>

      {/* ---- Hero ---- */}
      <main className="flex flex-1 flex-col items-center justify-center px-4 py-12">
        {/* Eyebrow */}
        <div className="mb-6 flex items-center gap-2">
          <span className="inline-block h-2 w-2 rounded-full bg-success pulse-dot" aria-hidden="true" />
          <span className="font-mono-tabular text-xs text-text-muted tracking-widest uppercase">
            Classic 3×3 Match
          </span>
        </div>

        {/* Title */}
        <h1 className="font-display text-5xl sm:text-6xl font-extrabold text-text-primary tracking-tight mb-3 text-center">
          TIC TAC TOE
        </h1>
        <p className="text-text-secondary text-base mb-10 text-center">
          Classic strategy. Modern interface.
        </p>

        {/* Mode selector card */}
        <div className="w-full max-w-2xl rounded-2xl border border-border bg-surface p-6 sm:p-8">
          <div className="mb-6 flex items-start justify-between">
            <div>
              <p className="font-mono-tabular text-xs text-text-muted tracking-widest uppercase mb-1">
                Select Match Type
              </p>
              <h2 className="font-display text-2xl font-bold text-text-primary">Choose your game</h2>
            </div>
            <p className="text-text-secondary text-sm hidden sm:block">
              Select a mode to start a new match.
            </p>
          </div>

          {/* Mode cards */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
            <ModeCard
              id="mode-two-player"
              label="MULTIPLAYER"
              badge="LOCAL"
              description="Play against another player locally on the same device with authoritative move checks."
              icon={<Users size={22} />}
              selected={selectedMode === 'TwoPlayer'}
              onSelect={() => setSelectedMode('TwoPlayer')}
            />
            <ModeCard
              id="mode-computer"
              label="VS COMPUTER"
              description="Challenge the computer"
              icon={<Monitor size={22} />}
              selected={selectedMode === 'Computer'}
              onSelect={() => setSelectedMode('Computer')}
            />
          </div>

          {/* Error */}
          {error && (
            <p role="alert" className="mb-4 rounded-lg border border-danger/30 bg-danger/10 px-4 py-2.5 text-sm text-danger">
              {error}
            </p>
          )}

          {/* Start button */}
          <Button
            id="start-game-btn"
            variant="primary"
            size="lg"
            className="w-full gap-2"
            isLoading={isStarting}
            onClick={handleStartGame}
            aria-label={`Start ${selectedMode === 'TwoPlayer' ? 'Two Player' : 'Vs Computer'} game`}
          >
            {!isStarting && <span>START GAME</span>}
            {!isStarting && <span aria-hidden="true">→</span>}
          </Button>

          <p className="mt-3 text-center font-mono-tabular text-xs text-text-muted">
            or load previous session via URL{' '}
            <span className="text-text-secondary">/game/:gameId</span>
          </p>
        </div>

        {/* Feature pills */}
        <div className="mt-10 flex flex-wrap justify-center gap-6 text-text-muted font-mono-tabular text-xs">
          <span className="flex items-center gap-1.5">
            <span aria-hidden="true">✓</span> Instant Move Validation
          </span>
          <span className="flex items-center gap-1.5">
            <span aria-hidden="true">⚡</span> Fast Engine REST API
          </span>
          <span className="flex items-center gap-1.5">
            <span aria-hidden="true">≡</span> Session Live Scoreboard
          </span>
        </div>
      </main>

      {/* ---- Footer ---- */}
      <footer className="flex items-center justify-between px-6 py-4 border-t border-border">
        <span className="font-mono-tabular text-xs text-text-muted flex items-center gap-1.5">
          <span className="inline-block h-1.5 w-1.5 rounded-full bg-success" aria-hidden="true" />
          Game Engine Ready
        </span>
        <span className="font-mono-tabular text-xs text-text-muted">
          Powered by React + .NET Web API
        </span>
      </footer>
    </div>
  )
}

// ------------------------------------------------------------------
// ModeCard — selectable game-mode card
// ------------------------------------------------------------------

interface ModeCardProps {
  id: string
  label: string
  badge?: string
  description: string
  icon: React.ReactNode
  selected: boolean
  onSelect: () => void
}

function ModeCard({ id, label, badge, description, icon, selected, onSelect }: ModeCardProps) {
  return (
    <button
      id={id}
      role="radio"
      aria-checked={selected}
      onClick={onSelect}
      className={[
        'relative flex flex-col gap-4 rounded-xl border p-5 text-left cursor-pointer',
        'transition-all duration-150 outline-none',
        'focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-surface',
        selected
          ? 'border-primary/50 bg-surface-raised shadow-[0_0_20px_-2px_rgba(124,92,252,0.18)]'
          : 'border-border bg-surface-raised hover:border-primary/30',
      ].join(' ')}
    >
      {/* Selected checkmark */}
      <div
        aria-hidden="true"
        className={[
          'absolute top-3 right-3 flex h-5 w-5 items-center justify-center rounded-full border transition-all',
          selected
            ? 'border-primary bg-primary text-text-primary'
            : 'border-border bg-transparent',
        ].join(' ')}
      >
        {selected && <span className="text-xs font-bold">✓</span>}
      </div>

      {/* Icon */}
      <div className={[
        'flex h-10 w-10 items-center justify-center rounded-xl',
        selected ? 'bg-primary text-text-primary' : 'bg-surface-elevated text-text-secondary',
        'transition-colors duration-150',
      ].join(' ')}>
        {icon}
      </div>

      {/* Labels */}
      <div>
        <div className="flex items-center gap-2 mb-1">
          <span className="font-display font-bold text-text-primary text-sm tracking-wide">
            {label}
          </span>
          {badge && (
            <span className="rounded px-1.5 py-0.5 text-[10px] font-mono-tabular font-semibold bg-surface-elevated text-text-muted border border-border">
              {badge}
            </span>
          )}
        </div>
        <p className="text-text-secondary text-xs leading-relaxed">{description}</p>
      </div>
    </button>
  )
}
