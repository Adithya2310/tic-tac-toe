import { BrowserRouter, Route, Routes } from 'react-router-dom'
import GamePage from './pages/GamePage'
import HomePage from './pages/HomePage'
import { AppRoute } from './constants/game'

/**
 * App — top-level router.
 *
 * Routes:
 *   /               → HomePage (game-mode selection)
 *   /game/:gameId   → GamePage (active game session)
 *
 * No additional routes are required by the assignment scope.
 */
export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path={AppRoute.home} element={<HomePage />} />
        <Route path={AppRoute.game} element={<GamePage />} />
      </Routes>
    </BrowserRouter>
  )
}
