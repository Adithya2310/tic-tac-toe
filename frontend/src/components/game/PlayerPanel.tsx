import type { GameState, PlayerSymbol } from '../../types/game'
import { GameMode, GameStatus, PlayerSymbol as PlayerSymbolValue } from '../../constants/game'

interface PlayerPanelProps {
  game: GameState
}

/**
 * PlayerPanel — shows both player cards with scores derived from the scoreboard.
 *
 * Active player card has a glowing 2px top accent line (from DESIGN.md §Player Matchup Card).
 * Scores are not shown here (scoreboard lives in the right panel), so we show
 * the game mode and waiting/thinking state.
 */
export default function PlayerPanel({ game }: PlayerPanelProps) {
  const isComputerMode = game.gameType === GameMode.Computer
  const currentPlayer = game.currentPlayer
  const isGameOver = game.status !== GameStatus.InProgress

  return (
    <div className="flex flex-col gap-3">
      {/* Label */}
      <div className="flex items-center justify-between">
        <span className="font-mono-tabular text-xs text-text-muted uppercase tracking-widest">Players</span>
        <span className="rounded-md border border-border bg-surface-raised px-2 py-0.5 font-mono-tabular text-xs text-text-muted">
          {isComputerMode ? 'vs Computer' : '1v1 Local'}
        </span>
      </div>

      {/* Player X card */}
      <PlayerCard
        symbol={PlayerSymbolValue.X}
        label={isComputerMode ? 'You' : 'Player 1'}
        subtitle={isComputerMode ? 'Human Player' : undefined}
        isActive={!isGameOver && currentPlayer === PlayerSymbolValue.X}
        isWinner={game.status === GameStatus.Won && game.winner === PlayerSymbolValue.X}
      />

      {/* Player O card */}
      <PlayerCard
        symbol={PlayerSymbolValue.O}
        label={isComputerMode ? 'Computer' : 'Player 2'}
        subtitle={isComputerMode ? 'AI Opponent' : undefined}
        isActive={!isGameOver && currentPlayer === PlayerSymbolValue.O}
        isWinner={game.status === GameStatus.Won && game.winner === PlayerSymbolValue.O}
        isThinking={isComputerMode && !isGameOver && currentPlayer === PlayerSymbolValue.O}
      />
    </div>
  )
}

// ------------------------------------------------------------------

interface PlayerCardProps {
  symbol: PlayerSymbol
  label: string
  subtitle?: string
  isActive: boolean
  isWinner: boolean
  isThinking?: boolean
}

function PlayerCard({ symbol, label, subtitle, isActive, isWinner, isThinking }: PlayerCardProps) {
  const isX = symbol === PlayerSymbolValue.X
  const accentColor = isX ? 'border-t-primary' : 'border-t-success'
  const symbolBg = isX ? 'bg-primary' : 'bg-success/20'
  const symbolText = isX ? 'text-text-primary' : 'text-success'

  return (
    <div
      className={[
        'rounded-xl border border-border bg-surface p-4',
        'transition-all duration-200',
        isActive ? `border-t-2 ${accentColor} shadow-[0_0_20px_-4px_rgba(124,92,252,0.2)]` : '',
        isWinner ? 'border-success/40' : '',
      ].join(' ')}
      aria-current={isActive ? 'true' : undefined}
    >
      <div className="flex items-center gap-3">
        {/* Symbol avatar */}
        <div className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-xl ${symbolBg}`}>
          <span className={`font-display font-bold text-lg ${isX ? 'text-text-primary' : symbolText}`}>
            {symbol}
          </span>
        </div>

        {/* Info */}
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2">
            <span className="font-display font-semibold text-sm text-text-primary truncate">{label}</span>
            {isWinner && <span className="text-xs text-warning">🏆</span>}
          </div>
          {subtitle && (
            <span className="text-xs text-text-muted">{subtitle}</span>
          )}
          {isActive && !isThinking && (
            <div className="flex items-center gap-1.5 mt-0.5">
              <span className="inline-block h-1.5 w-1.5 rounded-full bg-primary pulse-dot" aria-hidden="true" />
              <span className="text-xs text-text-secondary">Current Turn</span>
            </div>
          )}
          {isThinking && (
            <div className="mt-1 inline-flex items-center gap-1.5 rounded-full border border-border bg-surface-raised px-2.5 py-0.5">
              <span className="inline-block h-1.5 w-1.5 rounded-full bg-text-muted pulse-dot" aria-hidden="true" />
              <span className="font-mono-tabular text-xs text-text-muted">THINKING...</span>
            </div>
          )}
          {!isActive && !isThinking && !isWinner && (
            <span className="text-xs text-text-muted">Waiting move</span>
          )}
        </div>
      </div>
    </div>
  )
}
