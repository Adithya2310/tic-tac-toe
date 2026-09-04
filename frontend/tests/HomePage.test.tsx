import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter, useNavigate } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import HomePage from '../src/pages/HomePage'
import * as gameApi from '../src/services/gameApi'

vi.mock('../src/services/gameApi')

// Mock react-router-dom to spy on navigate
const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  }
})

describe('HomePage', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  it('renders both game modes', () => {
    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>
    )

    expect(screen.getByText('MULTIPLAYER')).toBeInTheDocument()
    expect(screen.getByText('VS COMPUTER')).toBeInTheDocument()
  })

  it('starts a game and navigates to the game page', async () => {
    const user = userEvent.setup()
    const mockCreateGame = vi.spyOn(gameApi, 'createGame').mockResolvedValue({
      gameId: 'mock-game-123',
      status: 'InProgress',
      gameType: 'TwoPlayer',
      currentPlayer: 'X',
      board: [['', '', ''], ['', '', ''], ['', '', '']],
      moves: [],
      winner: null,
      winningCells: []
    })

    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>
    )

    const startBtn = screen.getByRole('button', { name: /start two player game/i })
    await user.click(startBtn)

    expect(mockCreateGame).toHaveBeenCalledWith('TwoPlayer')
    expect(mockNavigate).toHaveBeenCalledWith('/game/mock-game-123')
  })

  it('shows error state if API fails', async () => {
    const user = userEvent.setup()
    vi.spyOn(gameApi, 'createGame').mockRejectedValue(new Error('Network Error'))

    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>
    )

    const startBtn = screen.getByRole('button', { name: /start two player game/i })
    await user.click(startBtn)

    expect(await screen.findByRole('alert')).toHaveTextContent('Network Error')
  })
})
