import type { GameState } from '../../types/game'

interface GameStatusProps {
  game: GameState
  isComputerThinking: boolean
  moveCount: number
}

/**
 * GameStatus — the dynamic status pill shown above the board.
 *
 * Possible states (from design-spec §3.2):
 *   X's Turn / O's Turn
 *   Computer is thinking...
 *   X Wins! / O Wins!
 *   It's a Draw
 */
export default function GameStatus({ game, isComputerThinking, moveCount }: GameStatusProps) {
  const { status, currentPlayer, winner } = game

  let statusText: string
  let textColor: string

  if (status === 'Won' && winner) {
    statusText = `${winner} Wins!`
    textColor = winner === 'X' ? 'text-primary' : 'text-success'
  } else if (status === 'Draw') {
    statusText = "It's a Draw"
    textColor = 'text-warning'
  } else if (isComputerThinking) {
    statusText = 'Computer is thinking...'
    textColor = 'text-text-secondary'
  } else {
    statusText = `Player ${currentPlayer}'s Turn`
    textColor = 'text-text-primary'
  }

  return (
    <div className="flex items-center justify-between gap-4 rounded-2xl border border-border bg-surface px-5 py-3.5">
      {/* Left: avatar + status */}
      <div className="flex items-center gap-3">
        {status === 'InProgress' && !isComputerThinking && (
          <div className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-lg ${currentPlayer === 'X' ? 'bg-primary' : 'bg-success/20'}`}>
            <span className={`font-display font-bold text-sm ${currentPlayer === 'X' ? 'text-text-primary' : 'text-success'}`}>
              {currentPlayer}
            </span>
          </div>
        )}
        <div>
          <p className={`font-display font-semibold text-sm ${textColor}`}>{statusText}</p>
          {status === 'InProgress' && !isComputerThinking && (
            <p className="text-xs text-text-muted">Select any available tile</p>
          )}
        </div>
      </div>

      {/* Right: move counter */}
      <div className="flex items-center gap-1.5 rounded-full border border-border bg-surface-raised px-3 py-1">
        <span className="font-mono-tabular text-xs text-text-muted">MOVE</span>
        <span className="font-mono-tabular text-xs font-semibold text-text-secondary">
          #{String(moveCount).padStart(2, '0')}
        </span>
      </div>
    </div>
  )
}
