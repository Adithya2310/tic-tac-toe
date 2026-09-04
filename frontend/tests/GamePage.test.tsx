import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import GamePage from '../src/pages/GamePage'
import * as gameApi from '../src/services/gameApi'

vi.mock('../src/services/gameApi')

const mockGameId = 'game-123'
const mockScoreboard = { xWins: 1, oWins: 0, draws: 0 }

const mockGameState = {
  gameId: mockGameId,
  status: 'InProgress',
  gameType: 'TwoPlayer',
  currentPlayer: 'X',
  board: [
    ['X', '', ''],
    ['', 'O', ''],
    ['', '', '']
  ],
  moves: [
    { moveNumber: 1, player: 'X', row: 0, column: 0 },
    { moveNumber: 2, player: 'O', row: 1, column: 1 }
  ],
  winner: null,
  winningCells: []
}

describe('GamePage', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  function renderGamePage() {
    return render(
      <MemoryRouter initialEntries={[`/game/${mockGameId}`]}>
        <Routes>
          <Route path="/game/:gameId" element={<GamePage />} />
        </Routes>
      </MemoryRouter>
    )
  }

  it('loads game using route ID and renders authoritative state', async () => {
    vi.spyOn(gameApi, 'getGame').mockResolvedValue(mockGameState as any)
    vi.spyOn(gameApi, 'getScoreboard').mockResolvedValue(mockScoreboard)

    renderGamePage()

    // Loading state initially
    expect(screen.getByText('Loading game…')).toBeInTheDocument()

    // Wait for load
    await waitFor(() => {
      expect(screen.queryByText('Loading game…')).not.toBeInTheDocument()
    })

    // Assert API was called
    expect(gameApi.getGame).toHaveBeenCalledWith(mockGameId)

    // Assert board state
    const cells = screen.getAllByRole('gridcell')
    expect(cells).toHaveLength(9)

    // X at (0,0)
    expect(cells[0]).toHaveAttribute('aria-label', 'Row 1, Column 1, occupied by X')
    expect(cells[0]).toBeDisabled()

    // Empty at (0,1)
    expect(cells[1]).toHaveAttribute('aria-label', 'Row 1, Column 2, empty cell')
    expect(cells[1]).not.toBeDisabled()

    // O at (1,1)
    expect(cells[4]).toHaveAttribute('aria-label', 'Row 2, Column 2, occupied by O')
    expect(cells[4]).toBeDisabled()
  })

  it('makes a move when an empty cell is clicked', async () => {
    const user = userEvent.setup()
    vi.spyOn(gameApi, 'getGame').mockResolvedValue(mockGameState as any)
    vi.spyOn(gameApi, 'getScoreboard').mockResolvedValue(mockScoreboard)
    const mockMakeMove = vi.spyOn(gameApi, 'makeMove').mockResolvedValue({
      ...mockGameState,
      board: [
        ['X', 'X', ''],
        ['', 'O', ''],
        ['', '', '']
      ],
      currentPlayer: 'O'
    } as any)

    renderGamePage()

    await screen.findByRole('grid') // wait for load

    const cells = screen.getAllByRole('gridcell')
    const emptyCell = cells[1] // (0,1)

    await user.click(emptyCell)

    expect(mockMakeMove).toHaveBeenCalledWith(mockGameId, 'X', 0, 1)
  })

  it('renders win modal when game state is Won', async () => {
    const wonState = {
      ...mockGameState,
      status: 'Won',
      winner: 'X',
      winningCells: [{ row: 0, column: 0 }, { row: 0, column: 1 }, { row: 0, column: 2 }]
    }
    vi.spyOn(gameApi, 'getGame').mockResolvedValue(wonState as any)
    vi.spyOn(gameApi, 'getScoreboard').mockResolvedValue(mockScoreboard)

    renderGamePage()

    await screen.findByRole('dialog', { name: /X WINS!/i })
    
    // Board is locked
    const cells = screen.getAllByRole('gridcell')
    cells.forEach(cell => {
      expect(cell).toBeDisabled()
    })
  })
})
