import { Home, RotateCcw, Trophy } from 'lucide-react'
import { useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import type { GameState } from '../../types/game'
import Button from '../ui/Button'

interface GameOverModalProps {
  game: GameState
  onReplay: () => void
  isResetting: boolean
}

/**
 * GameOverModal — shown when backend status becomes 'Won' or 'Draw'.
 *
 * Contents per frontend-guidelines §12:
 *   Win:  winner, result message, Go Home, Replay
 *   Draw: draw message, Go Home, Replay
 *
 * Out of scope: Export Move Log (per guidelines §38).
 *
 * Accessibility:
 *   - role="dialog", aria-modal="true"
 *   - focus trapped inside (first focusable element on open)
 *   - Escape key dismissal is intentionally omitted — the game must be
 *     replayed or user must go home to exit the completed state.
 */
export default function GameOverModal({ game, onReplay, isResetting }: GameOverModalProps) {
  const navigate = useNavigate()
  const firstButtonRef = useRef<HTMLButtonElement>(null)

  // Focus the first button when modal opens
  useEffect(() => {
    firstButtonRef.current?.focus()
  }, [])

  const isWin = game.status === 'Won'
  const isDraw = game.status === 'Draw'
  const winner = game.winner

  // Trophy icon style
  const trophyBg = isWin
    ? winner === 'X'
      ? 'bg-primary/20 shadow-[0_0_32px_rgba(124,92,252,0.25)]'
      : 'bg-success/20 shadow-[0_0_32px_rgba(52,211,153,0.25)]'
    : 'bg-warning/20 shadow-[0_0_32px_rgba(251,191,36,0.25)]'

  const trophyColor = isWin
    ? winner === 'X' ? 'text-primary' : 'text-success'
    : 'text-warning'

  const titleText = isWin ? `${winner} WINS!` : "IT'S A DRAW"
  const subtitleText = isWin
    ? winner === 'X' ? 'Three in a row! Great game.' : 'Computer wins this round!'
    : 'No winner this time. Play again?'

  const shortId = game.gameId.slice(0, 8).toUpperCase()

  return (
    /* Backdrop */
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4"
      style={{ background: 'rgba(11,13,18,0.80)', backdropFilter: 'blur(8px)' }}
      aria-hidden="false"
    >
      {/* Modal panel */}
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-title"
        className="modal-in w-full max-w-md rounded-2xl border border-border bg-surface-raised shadow-2xl"
      >
        <div className="flex flex-col items-center px-8 py-8 gap-6">
          {/* Trophy icon */}
          <div className={`relative flex h-20 w-20 items-center justify-center rounded-2xl ${trophyBg}`}>
            <Trophy size={40} className={trophyColor} aria-hidden="true" />
            {isWin && (
              <span
                aria-hidden="true"
                className="absolute -bottom-1.5 -right-1.5 flex h-6 w-6 items-center justify-center rounded-full bg-primary text-text-primary text-xs font-bold shadow"
              >
                ✓
              </span>
            )}
          </div>

          {/* Title + subtitle */}
          <div className="text-center">
            <h2 id="modal-title" className="font-display text-3xl font-extrabold text-text-primary mb-1.5">
              {titleText}
            </h2>
            <p className="text-text-secondary text-sm">{subtitleText}</p>
          </div>

          {/* Match stats */}
          <div className="w-full rounded-xl border border-border bg-surface px-4 py-3 space-y-2">
            <div className="flex items-center justify-between text-xs">
              <span className="font-mono-tabular text-text-muted">Match ID</span>
              <span className="font-mono-tabular text-text-secondary">#{shortId}</span>
            </div>
            <div className="border-t border-border" />
            <div className="grid grid-cols-2 gap-4 pt-1">
              <div className="text-center">
                <p className="font-mono-tabular text-xs text-text-muted mb-0.5">Total Moves</p>
                <p className="font-mono-tabular font-semibold text-text-primary">
                  {game.moves.length} {game.moves.length === 1 ? 'Move' : 'Moves'}
                </p>
              </div>
              {isWin && (
                <div className="text-center">
                  <p className="font-mono-tabular text-xs text-text-muted mb-0.5">Winner</p>
                  <p className={`font-mono-tabular font-semibold ${winner === 'X' ? 'text-primary' : 'text-success'}`}>
                    {winner === 'X' ? 'Player 1 (X)' : game.gameType === 'Computer' ? 'Computer (O)' : 'Player 2 (O)'}
                  </p>
                </div>
              )}
              {isDraw && (
                <div className="text-center">
                  <p className="font-mono-tabular text-xs text-text-muted mb-0.5">Result</p>
                  <p className="font-mono-tabular font-semibold text-warning">Draw</p>
                </div>
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="flex w-full gap-3">
            <Button
              ref={firstButtonRef}
              id="modal-go-home-btn"
              variant="secondary"
              size="lg"
              className="flex-1 gap-2"
              onClick={() => navigate('/')}
            >
              <Home size={16} aria-hidden="true" />
              Go Home
            </Button>
            <Button
              id="modal-replay-btn"
              variant="primary"
              size="lg"
              className="flex-1 gap-2"
              isLoading={isResetting}
              onClick={onReplay}
              aria-label="Replay — reset the current game"
            >
              {!isResetting && <RotateCcw size={16} aria-hidden="true" />}
              {!isResetting && 'Replay'}
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
