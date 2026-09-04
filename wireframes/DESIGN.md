

## Brand & Style

This design system establishes a high-performance, developer-first aesthetic tailored for strategic, real-time web applications. Rooted in the visual vernacular of modern engineering tools, the aesthetic combines mathematical discipline, spatial clarity, and tactile digital surfaces.

The interface prioritizes focus, deliberate feedback, and technical confidence. It avoids gamified visual clutter in favor of crisp borders, layered dark substrates, precise monospaced indicators, and vivid semantic illumination. Interactions are calibrated to feel immediate, deterministic, and satisfying—treating a classic board state like a mission-critical developer console.

## Colors

The palette leverages a deep spectral hierarchy starting from an obsidian void, building structural depth through finely tuned intermediate dark surfaces, and accented with targeted luminescent states.

### Core Architecture
- **Canvas Base (`#0B0D12`)**: The foundational deep obsidian backplane.
- **Surface Level 1 (`#131722`)**: Neutral background for panels, side drawers, and game board housing.
- **Surface Level 2 (`#191E2A`)**: Raised cards, active cells, score counters, and dropdown dialogs.
- **Surface Level 3 / Overlay (`#212838`)**: Hover-state overlays, popovers, and elevated modals.
- **Border / Structural (`#282E3A`)**: Subtle 1px dividing lines, cell demarcations, and component boundaries.
- **Border Subtle (`rgba(255, 255, 255, 0.06)`)**: Hairline inner highlights for glass and elevation effects.

### Content Tones
- **Text Primary (`#F5F7FA`)**: Maximum legibility for scores, active player states, and critical headlines.
- **Text Secondary (`#9AA3B2`)**: Supporting labels, turn history metadata, and inactive elements.
- **Text Muted (`#64748B`)**: Coordinates, system timestamps, and disabled controls.

### Functional Accents
- **Primary Accent (`#7C5CFC`)**: Brand focal points, 'X' player motifs, primary button fills, and keyboard focus halos.
- **Secondary Accent (`#A78BFA`)**: Sub-elements, secondary hover highlights, and auxiliary active indicators.
- **Success / Victory (`#34D399`)**: Winning alignment lines, 'O' player motifs, and victory banner fills.
- **Warning / Standoff (`#FBBF24`)**: Draw outcomes, latency alerts, and tie indicator chips.
- **Danger / Reset (`#F87171`)**: Resignation dialogs, error indicators, and forfeit actions.

## Typography

The typographic hierarchy pairs crisp sans-serif letterforms with a high-density monospaced font for computational telemetry.

- **Plus Jakarta Sans** delivers structural balance for modal titles, board outcomes, and dashboard headers with tight geometric tracking.
- **Inter** provides neutral, effortless legibility across rules, dynamic status copy, and control labels.
- **JetBrains Mono** houses all machine-derived metrics: game session IDs, turn sequence indexes, grid coordinates (e.g., `[0, 2]`), match durations, and real-time score values. Monospace figures must strictly preserve tabular lining (`font-variant-numeric: tabular-nums`).

## Layout & Spacing

The spatial model employs an 8-point base grid system with a 4-point micro-subgrid for small components like tags, coordinates, and badge insets.

### Application Layout Archetype
- **Desktop (>=1024px)**: Fixed-width central arena of `1120px` max-width. Employs an asymmetric 3-column composition:
  - Left utility deck (`280px`): Session metadata, player card `X`.
  - Center stage (`560px`): Dynamic status pill, primary 3x3 game board container, quick action bar.
  - Right telemetry deck (`280px`): Chronological move feed, player card `O`.
- **Tablet (768px - 1023px)**: Two-column configuration. The game arena remains dominant on the left (`60%`), while history and telemetry stack vertically on the right (`40%`).
- **Mobile (<768px)**: Single-column vertical stream. The board occupies a fixed aspect-ratio container (`min(calc(100vw - 2rem), 400px)`), with collapsed expandable drawers for match history.

### Board Geometry
The 3x3 grid maintains a rigid square aspect ratio. Cells are spaced consistently using `board-gap` (`12px`), nested inside an insulated `#131722` housing with `1.5rem` internal clearance.

## Elevation & Depth

Visual depth is achieved through high-precision tonal stacking, dark-mode glassmorphism, and colored edge diffusion rather than heavy physical dropshadows.

### Layer Stack
1. **Layer 0 (Canvas Base)**: `#0B0D12` with an optional subtle CSS radial gradient (`radial-gradient(ellipse at 50% 0%, #1A1F30 0%, #0B0D12 75%)`).
2. **Layer 1 (Card & Board Substrate)**: Solid `#131722` with a 1px solid border of `#282E3A`.
3. **Layer 2 (Elevated Cells, Chips, Controls)**: `#191E2A` accompanied by an ambient shadow: `0 4px 16px -4px rgba(0, 0, 0, 0.45)`.
4. **Layer 3 (Overlays, Modals, Flyouts)**: Translucent `#191E2A` at 90% opacity paired with `backdrop-filter: blur(12px)` and a subtle light shelf: `inset 0 1px 0 0 rgba(255, 255, 255, 0.08)`. Border shifts to `#3B4455`.

### Focus & Active Glow
- Cell hover: `box-shadow: 0 0 20px -2px rgba(124, 92, 252, 0.18)`
- Win condition highlights: Pulsing radial outer glow using the winning token's semantic hue: `0 0 32px 2px rgba(52, 211, 153, 0.25)`

## Shapes

The design system pairs refined, medium-large radii with sharp internal component alignment.

- **Standard Elements (`rounded-lg` / 16px)**: Used on elevated cards, secondary containers, and modal dialog bodies.
- **Board Arena & Grid Cells (`rounded-xl` / 24px)**: Outer game board frame utilizes 24px radius, while individual interactive grid cells utilize 16px radius, creating a nested curvature ratio.
- **Micro-Components (`rounded-md` / 8px)**: Badges, coordinate tags, activity feed items, and segmented controls.
- **Status Indicators & Turn Pills (`rounded-full` / 9999px)**: Full pill geometry for player turn notifications, status pills, and counter tags.

## Components

### Buttons
- **Primary**: Background `#7C5CFC`, text `#F5F7FA`, 1px border `rgba(255, 255, 255, 0.1)`. Hover shifts to `#6B4CE6` with `0 0 16px rgba(124, 92, 252, 0.35)`. Active scale drops to `0.98`.
- **Secondary (Outline)**: Background `transparent`, border 1px solid `#282E3A`, text `#F5F7FA`. Hover applies background `#191E2A` and border `#3B4455`.
- **Ghost**: Background `transparent`, border `transparent`, text `#9AA3B2`. Hover shifts text to `#F5F7FA` with background `rgba(255, 255, 255, 0.04)`.
- **Icon / Utility**: Strict 1:1 aspect ratio (36x36px or 40x40px), rounded-lg, centered SVGs with 1.5px stroke width.

### Game Board Cells
- Dimension: Uniform square with smooth cubic-bezier transitions (`transition: all 180ms cubic-bezier(0.16, 1, 0.3, 1)`).
- **Default State**: Surface `#191E2A`, border 1px solid `#282E3A`. Display faint coordinates in top-left using `label-code-xs` (`#64748B`).
- **Hover (Unoccupied)**: Border `#7C5CFC` (or current turn color) at 50% opacity, background `#1F2636`, with a muted preview token at 30% opacity.
- **Occupied State**: Cursor `not-allowed`. SVG glyph scaled to 48% cell width with stroke weight of 8px.
  - `X` Glyph: Colored `#7C5CFC` with subtle drop shadow `0 0 12px rgba(124, 92, 252, 0.4)`.
  - `O` Glyph: Colored `#34D399` with subtle drop shadow `0 0 12px rgba(52, 211, 153, 0.4)`.
- **Winning Path**: Cell background `#191E2A`, border 1px solid currentColor, accompanied by a dynamic pulsing perimeter glow and elevated scale `1.02`.

### Status Badges & Turn Pills
- **Active Turn Pill**: Pill-shaped container (`rounded-full`) with surface `#131722`, border 1px solid `#282E3A`, internal dot indicator (8px) pulsing with current player's color, accompanied by `label-code-md` typography.
- **Player Matchup Card**: Surface `#131722`, border 1px solid `#282E3A`. Active player card reveals a glowing 2px top accent line matching their assigned color.

### Scoreboard Counters
- Structured tabular layouts displaying Player X, Tie, and Player O tallies.
- Numerical displays use `label-code-lg` at 24px weight, rendered in `#F5F7FA` with a subtle `#191E2A` inset capsule container.

### Activity Feed (Move Ledger)
- Chronological list rendered in reverse-order (latest move at the top).
- Rows consist of:
  - Step indicator `#01`, `#02` in `#64748B`.
  - Player avatar/token pill (`X` or `O`).
  - Board grid matrix notation `row: 1, col: 2` in `label-code-xs`.
  - Real-time timestamp or turn delta in `#9AA3B2`.
- Border-left connected timeline track with a 1px dashed line of `#282E3A`.

### Modal Dialogs (Victory / Standoff)
- Surface `#131722` at 95% opacity with 20px blur backdrop. Outer border 1px solid `#3B4455`.
- Modal header features an illuminated icon halo corresponding to state (Emerald for Victory, Amber for Standoff, Rose for Defeat).
- Primary actions laid out horizontally: "Play Again" (Primary fill), "Export Move Log" (Secondary outline).