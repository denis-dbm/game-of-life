using static GameOfLife.Core.Liveness;

namespace GameOfLife.Core;

/// <summary>
/// Represents a board of cells in the Game of Life.
/// </summary>
/// <remarks>
/// The design uses a stateful approach to prioritize performance and memory efficiency (less objects in Managed Heap, avoid GC pressure, and so on).
/// The approach attends intensive creation of generations which can demand intensive computational resources.
/// 
/// In some cases, the state (object reference) cannot be shared to avoid error-prone client code. That is the case of the <see cref="Cells"/> method.
/// </remarks>
public class Board
{
    private SortedSet<LivenessBoardCell> _cells;
    private SortedSet<LivenessBoardCell> _nextGenCells = [];

    public BoardId Id { get; private set; }
    
    public long Generation { get; private set; }

    public IReadOnlySet<LivenessBoardCell> Cells
    {
        get => new SortedSet<LivenessBoardCell>(_cells);
        private set => _cells = [..value.Where(c => c.State is Alive)];
    }

    public bool HasMutatedFromLastGeneration { get; private set; }

    public int Population => _cells.Count;

    public Board(BoardId id, long generation, IReadOnlySet<LivenessBoardCell> cells)
    {
        Id = id;
        Generation = generation > -1 ? generation : throw new ArgumentOutOfRangeException(nameof(generation), "Generation must be greater than or equal to 0.");
        Cells = cells;
    }

    public long NextGeneration() => NextGeneration(false);

    public long NextGeneration(bool freezeOnNonMutation)
    {
        _nextGenCells.Clear();
        var neighborhoodVisitor = new NeighborhoodVisitor(_cells, _nextGenCells);
        
        foreach (var cell in _cells)
        {
            cell.VisitNeighborhood(ref neighborhoodVisitor);
            neighborhoodVisitor.Reset();
        }

        bool mutated = !_nextGenCells.SetEquals(_cells);

        if (!mutated && freezeOnNonMutation)
            return Generation;
        
        HasMutatedFromLastGeneration = mutated;
        (_cells, _nextGenCells) = (_nextGenCells, _cells);
        return ++Generation;
    }

    /// <summary>
    /// Internal only: forces a generation when calls to <see cref="NextGeneration"/> do not mutate the board anymore.
    /// </summary>
    /// <param name="generation"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SetGeneration(long generation)
    {
        if (generation < 0)
            throw new ArgumentOutOfRangeException(nameof(generation), "Generation must be greater than or equal to 0.");

        if (HasMutatedFromLastGeneration)
            throw new InvalidOperationException("Cannot set generation when the board has mutated from the last generation.");

        Generation = generation;
    }
}

/// <summary>
/// Implements the Visitor Pattern to calculate the next generation of cells.
/// </summary>
/// <param name="cells">Current cells in the board.</param>
/// <param name="nextGenCells">A <see cref="SortedSet{T}"/> to hold the calculated next generation of cells.</param>
file struct NeighborhoodVisitor(SortedSet<LivenessBoardCell> cells, SortedSet<LivenessBoardCell> nextGenCells) : INeighborhoodVisitor
{
    private int _aliveNeighbors;
    private int? _nestedAliveNeighbors;

    public void Visit(long x, long y)
    {
        var cell = new LivenessBoardCell(x, y);

        if (cells.TryGetValue(cell, out var neighbor) && neighbor.State is Alive)
            if (_nestedAliveNeighbors is null)
                _aliveNeighbors += 1;
            else
                _nestedAliveNeighbors += 1;

        if (_nestedAliveNeighbors is null)
        {
            _nestedAliveNeighbors = 0;
            cell.VisitNeighborhood(ref this);

            if (IsAlive(_nestedAliveNeighbors!.Value, neighbor.State))
                nextGenCells.Add(new(x, y, Alive));

            _nestedAliveNeighbors = null;
        }
    }

    public readonly void Done(long originalX, long originalY)
    {
        if (_nestedAliveNeighbors is not null)
            return;

        cells.TryGetValue(new(originalX, originalY), out var cell);

        if (IsAlive(_aliveNeighbors, cell.State))
            nextGenCells.Add(new(originalX, originalY, Alive));
    }

    private static bool IsAlive(int aliveNeighbors, Liveness currentState) => 
        (aliveNeighbors is 2 or 3 && currentState is Alive) || (aliveNeighbors is 3 && currentState is Dead);

    public void Reset() => _aliveNeighbors = 0;
}