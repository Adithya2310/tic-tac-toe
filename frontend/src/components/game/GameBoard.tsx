import type { GameState, PlayerSymbol, Position } from '../../types/game'

interface BoardCellProps {
  row: number
  column: number
  value: PlayerSymbol | ''
  isWinningCell: boolean
  isInteractive: boolean
  isSubmitting: boolean
  currentPlayer: PlayerSymbol
  onClick: (row: number, column: number) => void
}

/**
 * BoardCell — a single 3×3 grid cell.
 *
 * All display state is derived from props (authoritative backend values).
 * The frontend never computes winning cells — it reads winningCells from GameState.
 *
 * States (from DESIGN.md §Game Board Cells):
 *   empty + interactive — hover border, faint preview, focus ring
 *   occupied X — purple SVG glyph with glow
 *   occupied O — green SVG glyph with glow
 *   winning cell — pulsing glow + scale 1.02
 *   disabled — not-allowed cursor, no hover
 */
export default function BoardCell({
  row,
  column,
  value,
  isWinningCell,
  isInteractive,
  isSubmitting,
  currentPlayer,
  onClick,
}: BoardCellProps) {
  const isEmpty = value === ''
  const isX = value === 'X'
  const isO = value === 'O'

  // Determine hover preview color based on current player
  const hoverBorderColor = currentPlayer === 'X' ? 'hover:border-primary/50' : 'hover:border-success/50'
  const hoverBg = 'hover:bg-[#1F2636]'

  const cellClasses = [
    'relative flex items-center justify-center',
    'rounded-2xl border border-border bg-surface-raised',
    'transition-all duration-[180ms] cubic-bezier(0.16,1,0.3,1)',
    'aspect-square w-full',
    'outline-none',
    // Interactive states
    isInteractive && isEmpty
      ? `cursor-pointer ${hoverBorderColor} ${hoverBg} hover:shadow-[0_0_20px_-2px_rgba(124,92,252,0.18)] focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-background`
      : 'cursor-not-allowed',
    // Winning cell
    isWinningCell
      ? isX
        ? 'border-primary/70 cell-winning-x scale-[1.02]'
        : 'border-success/70 cell-winning scale-[1.02]'
      : '',
  ].filter(Boolean).join(' ')

  // Coordinate label (top-left, faint — from DESIGN.md)
  const coordLabel = `${row + 1},${column + 1}`

  return (
    <button
      role="gridcell"
      aria-label={
        isEmpty
          ? `Row ${row + 1}, Column ${column + 1}, empty cell`
          : `Row ${row + 1}, Column ${column + 1}, occupied by ${value}`
      }
      aria-disabled={!isInteractive || isSubmitting}
      disabled={!isInteractive || isSubmitting}
      onClick={() => isInteractive && isEmpty && !isSubmitting && onClick(row, column)}
      className={cellClasses}
    >
      {/* Coordinate label — visible when empty */}
      {isEmpty && (
        <span
          aria-hidden="true"
          className="absolute top-2 left-2.5 font-mono-tabular text-[10px] text-text-muted opacity-60 select-none"
        >
          {coordLabel}
        </span>
      )}

      {/* X glyph */}
      {isX && (
        <XGlyph
          isWinning={isWinningCell}
          className="mark-appear"
        />
      )}

      {/* O glyph */}
      {isO && (
        <OGlyph
          isWinning={isWinningCell}
          className="mark-appear"
        />
      )}

      {/* Empty hover preview — faint current-player marker */}
      {isEmpty && isInteractive && (
        <span
          aria-hidden="true"
          className={[
            'absolute inset-0 flex items-center justify-center opacity-0 pointer-events-none',
            'transition-opacity duration-150',
            currentPlayer === 'X' ? 'hover-show-x' : 'hover-show-o',
          ].join(' ')}
        />
      )}
    </button>
  )
}

// ------------------------------------------------------------------
// SVG glyphs — rendered as inline SVG for full styling control.
// Stroke weight: 8px equivalent. Size: 48% of cell.
// ------------------------------------------------------------------

function XGlyph({ isWinning, className }: { isWinning: boolean; className?: string }) {
  return (
    <svg
      viewBox="0 0 48 48"
      className={`w-[48%] h-[48%] ${className ?? ''}`}
      aria-hidden="true"
    >
      <line
        x1="8" y1="8" x2="40" y2="40"
        stroke={isWinning ? '#A78BFA' : '#7C5CFC'}
        strokeWidth="7"
        strokeLinecap="round"
        style={{ filter: 'drop-shadow(0 0 8px rgba(124,92,252,0.5))' }}
      />
      <line
        x1="40" y1="8" x2="8" y2="40"
        stroke={isWinning ? '#A78BFA' : '#7C5CFC'}
        strokeWidth="7"
        strokeLinecap="round"
        style={{ filter: 'drop-shadow(0 0 8px rgba(124,92,252,0.5))' }}
      />
    </svg>
  )
}

function OGlyph({ isWinning, className }: { isWinning: boolean; className?: string }) {
  return (
    <svg
      viewBox="0 0 48 48"
      className={`w-[48%] h-[48%] ${className ?? ''}`}
      aria-hidden="true"
    >
      <circle
        cx="24" cy="24" r="16"
        fill="none"
        stroke={isWinning ? '#6EE7B7' : '#34D399'}
        strokeWidth="7"
        strokeLinecap="round"
        style={{ filter: 'drop-shadow(0 0 8px rgba(52,211,153,0.5))' }}
      />
    </svg>
  )
}

// ------------------------------------------------------------------
// GameBoard — 3×3 grid of BoardCells
// ------------------------------------------------------------------

interface GameBoardProps {
  game: GameState
  isSubmitting: boolean
  isComputerThinking: boolean
  onCellClick: (row: number, column: number) => void
}

export function GameBoard({ game, isSubmitting, isComputerThinking, onCellClick }: GameBoardProps) {
  const isGameOver = game.status !== 'InProgress'
  const boardInteractive = !isGameOver && !isSubmitting && !isComputerThinking

  // Build a Set for fast winning-cell lookups
  const winningSet = new Set(
    game.winningCells.map((p: Position) => `${p.row},${p.column}`)
  )

  return (
    <div
      role="grid"
      aria-label="Tic Tac Toe board"
      className={[
        'relative rounded-3xl border border-border bg-surface p-5',
        'shadow-[0_4px_24px_-4px_rgba(0,0,0,0.6)]',
        isComputerThinking ? 'opacity-50' : '',
        'transition-opacity duration-300',
      ].join(' ')}
    >
      <div
        className="grid grid-cols-3"
        style={{ gap: '12px' }}
      >
        {game.board.map((row, ri) =>
          row.map((cell, ci) => {
            const cellValue = cell === '' ? '' : (cell as PlayerSymbol)
            const isWinning = winningSet.has(`${ri},${ci}`)
            return (
              <BoardCell
                key={`${ri}-${ci}`}
                row={ri}
                column={ci}
                value={cellValue}
                isWinningCell={isWinning}
                isInteractive={boardInteractive && cell === ''}
                isSubmitting={isSubmitting}
                currentPlayer={game.currentPlayer}
                onClick={onCellClick}
              />
            )
          })
        )}
      </div>

      {/* Computer thinking overlay */}
      {isComputerThinking && (
        <div
          aria-live="polite"
          aria-label="Computer is thinking"
          className="absolute inset-0 flex items-center justify-center rounded-3xl"
        >
          <div className="flex items-center gap-2 rounded-full border border-border bg-surface/90 px-4 py-2 backdrop-blur-sm">
            <span className="inline-block h-2 w-2 rounded-full bg-text-muted pulse-dot" aria-hidden="true" />
            <span className="font-mono-tabular text-sm text-text-secondary">Computer is thinking...</span>
          </div>
        </div>
      )}
    </div>
  )
}
