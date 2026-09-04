import { History, RotateCcw } from 'lucide-react'
import type { Move, ScoreboardState } from '../../types/game'

// ------------------------------------------------------------------
// MoveActivity — move history feed
// ------------------------------------------------------------------

interface MoveActivityProps {
  moves: Move[]
}

/**
 * MoveActivity — chronological move ledger, latest move at top.
 *
 * Renders the backend's moves[] collection directly.
 * Does not maintain a second client-side history.
 * Format: `#01  X  Row 1 • Column 1`
 */
export function MoveActivity({ moves }: MoveActivityProps) {
  const reversed = [...moves].reverse()

  return (
    <div className="flex flex-col rounded-2xl border border-border bg-surface">
      <div className="flex items-center justify-between px-4 py-3 border-b border-border">
        <div className="flex items-center gap-2">
          <History size={14} className="text-text-muted" aria-hidden="true" />
          <span className="font-mono-tabular text-xs text-text-muted uppercase tracking-widest">
            Move Ledger
          </span>
        </div>
        <span className="font-mono-tabular text-xs text-text-muted">
          {moves.length} {moves.length === 1 ? 'move' : 'moves'}
        </span>
      </div>

      <div className="flex flex-col overflow-y-auto max-h-72 divide-y divide-border">
        {reversed.length === 0 ? (
          <p className="px-4 py-4 text-xs text-text-muted text-center font-mono-tabular">
            No moves yet
          </p>
        ) : (
          reversed.map((move, idx) => (
            <MoveRow
              key={move.moveNumber}
              move={move}
              isLatest={idx === 0}
            />
          ))
        )}
      </div>
    </div>
  )
}

interface MoveRowProps {
  move: Move
  isLatest: boolean
}

function MoveRow({ move, isLatest }: MoveRowProps) {
  const isX = move.player === 'X'
  const symbolBg = isX ? 'bg-primary/20 text-primary' : 'bg-success/20 text-success'

  return (
    <div className={`flex items-center gap-3 px-4 py-2.5 ${isLatest ? 'bg-surface-raised' : ''}`}>
      {/* Step indicator */}
      <span className="font-mono-tabular text-xs text-text-muted w-5 shrink-0">
        #{String(move.moveNumber).padStart(2, '0')}
      </span>

      {/* Player pill */}
      <span className={`flex h-6 w-6 shrink-0 items-center justify-center rounded-md font-display font-bold text-xs ${symbolBg}`}>
        {move.player}
      </span>

      {/* Position */}
      <span className="flex-1 font-mono-tabular text-xs text-text-secondary">
        Row {move.row + 1} • Col {move.column + 1}
      </span>

      {/* Latest badge */}
      {isLatest && (
        <span className="rounded px-1.5 py-0.5 font-mono-tabular text-[10px] font-semibold border border-primary/30 bg-primary/10 text-primary">
          LATEST
        </span>
      )}
    </div>
  )
}

// ------------------------------------------------------------------
// Scoreboard — session score display
// ------------------------------------------------------------------

interface ScoreboardProps {
  scoreboard: ScoreboardState
  isResetting: boolean
  onReset: () => void
}

/**
 * Scoreboard — session-level X wins / Draws / O wins.
 *
 * Values come directly from the backend.
 * The frontend never calculates scoreboard totals.
 */
export function Scoreboard({ scoreboard, isResetting, onReset }: ScoreboardProps) {
  return (
    <div className="flex flex-col rounded-2xl border border-border bg-surface">
      <div className="flex items-center justify-between px-4 py-3 border-b border-border">
        <span className="font-mono-tabular text-xs text-text-muted uppercase tracking-widest">
          Scoreboard
        </span>
        <button
          id="reset-scoreboard-btn"
          onClick={onReset}
          disabled={isResetting}
          className="flex items-center gap-1 text-text-muted text-xs hover:text-text-secondary transition-colors disabled:opacity-40 focus-visible:ring-1 focus-visible:ring-primary rounded"
          aria-label="Reset scoreboard"
        >
          <RotateCcw size={11} aria-hidden="true" />
          Reset
        </button>
      </div>

      <div className="grid grid-cols-3 divide-x divide-border">
        <ScoreCell label="YOU (X)" value={scoreboard.xWins} color="text-primary" />
        <ScoreCell label="DRAWS" value={scoreboard.draws} color="text-warning" />
        <ScoreCell label="BOT (O)" value={scoreboard.oWins} color="text-success" />
      </div>
    </div>
  )
}

interface ScoreCellProps {
  label: string
  value: number
  color: string
}

function ScoreCell({ label, value, color }: ScoreCellProps) {
  return (
    <div className="flex flex-col items-center py-4 px-2 gap-1">
      <span className="font-mono-tabular text-[10px] text-text-muted uppercase tracking-wide">{label}</span>
      <span className={`font-mono-tabular text-2xl font-semibold ${color}`}>
        {value}
      </span>
    </div>
  )
}
