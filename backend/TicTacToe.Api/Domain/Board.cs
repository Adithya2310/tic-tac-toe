namespace TicTacToe.Api.Domain;

/// <summary>
/// Manages the 3x3 board state.
/// Board owns cell-level constraints; it does not own turn logic or win detection.
/// </summary>
public sealed class Board
{
    private const int Size = 3;
    private readonly Symbol[,] _cells;

    public Board()
    {
        _cells = new Symbol[Size, Size];
    }

    private Board(Symbol[,] cells)
    {
        _cells = cells;
    }

    /// <summary>Returns true when the position is within the 3x3 grid.</summary>
    public bool IsValidPosition(Position position) =>
        position.Row >= 0 && position.Row < Size &&
        position.Column >= 0 && position.Column < Size;

    /// <summary>Returns true when the target cell has no mark placed on it.</summary>
    public bool IsCellEmpty(Position position) =>
        _cells[position.Row, position.Column] == Symbol.Empty;

    /// <summary>Places the given symbol at the position. Assumes the position is valid and empty.</summary>
    public void PlaceMark(Position position, Symbol symbol)
    {
        _cells[position.Row, position.Column] = symbol;
    }

    /// <summary>Returns the symbol at the given position.</summary>
    public Symbol GetCell(Position position) =>
        _cells[position.Row, position.Column];

    /// <summary>Returns all positions that currently contain no mark.</summary>
    public IReadOnlyList<Position> GetEmptyPositions()
    {
        var result = new List<Position>();
        for (var r = 0; r < Size; r++)
        for (var c = 0; c < Size; c++)
            if (_cells[r, c] == Symbol.Empty)
                result.Add(new Position(r, c));
        return result;
    }

    /// <summary>Returns true when every cell has a mark placed on it.</summary>
    public bool IsFull() => GetEmptyPositions().Count == 0;

    /// <summary>Resets every cell to Empty.</summary>
    public void Clear()
    {
        for (var r = 0; r < Size; r++)
        for (var c = 0; c < Size; c++)
            _cells[r, c] = Symbol.Empty;
    }

    /// <summary>
    /// Returns a shallow copy of this board so that the strategy can evaluate
    /// positions without mutating the real game board.
    /// </summary>
    public Board Clone()
    {
        var copy = new Symbol[Size, Size];
        Array.Copy(_cells, copy, _cells.Length);
        return new Board(copy);
    }

    /// <summary>
    /// Returns the full board as a read-only 2D representation for serialisation.
    /// Row-major order: result[row][column].
    /// </summary>
    public IReadOnlyList<IReadOnlyList<Symbol>> ToGrid()
    {
        var rows = new List<IReadOnlyList<Symbol>>(Size);
        for (var r = 0; r < Size; r++)
        {
            var row = new Symbol[Size];
            for (var c = 0; c < Size; c++)
                row[c] = _cells[r, c];
            rows.Add(row);
        }
        return rows;
    }
}
