import { RotateCcw, Undo2 } from 'lucide-react'
import Button from '../ui/Button'

interface GameControlsProps {
  canUndo: boolean
  isSubmitting: boolean
  onUndo: () => void
  onReset: () => void
}

/**
 * GameControls — Undo Last Move + Reset Game buttons below the board.
 *
 * Undo availability is determined by the useGame hook from authoritative state.
 * The frontend never independently decides whether undo is valid.
 */
export default function GameControls({ canUndo, isSubmitting, onUndo, onReset }: GameControlsProps) {
  return (
    <div className="flex gap-3">
      <Button
        id="undo-move-btn"
        variant="secondary"
        size="lg"
        className="flex-1 gap-2"
        disabled={!canUndo || isSubmitting}
        onClick={onUndo}
        aria-label="Undo last move"
      >
        <Undo2 size={16} aria-hidden="true" />
        Undo Move
      </Button>

      <Button
        id="reset-game-btn"
        variant="primary"
        size="lg"
        className="flex-1 gap-2"
        disabled={isSubmitting}
        isLoading={isSubmitting}
        onClick={onReset}
        aria-label="Reset game"
      >
        {!isSubmitting && <RotateCcw size={16} aria-hidden="true" />}
        New Game
      </Button>
    </div>
  )
}
