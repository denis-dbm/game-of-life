using System.Diagnostics;

namespace GameOfLife.Core;

/// <summary>
/// Represents a cell on a board in the Game of Life.
/// Each cell can have a state of type <typeparamref name="TState"/>.
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <remarks>
/// <see cref="State" /> is just a holder for a value: it does not participate in equality rules.
/// This design allows the use of Sets instead of the requirement of a dictionary to minimize memory consumption.
/// </remarks>
[DebuggerDisplay("({X}, {Y}, {State})")]
public readonly struct BoardCell<TState> : IComparable<BoardCell<TState>>
{
    public long X { get; }
    public long Y { get; }
    public TState? State { get; }

    public BoardCell(long x, long y, TState? state = default)
    {
        X = x;
        Y = y;
        State = state;
    }

    public override bool Equals(object? obj) => obj is BoardCell<TState> cell && X == cell.X && Y == cell.Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public int CompareTo(BoardCell<TState> other)
    {
        var xComparison = X.CompareTo(other.X);
        return xComparison != 0 ? xComparison : Y.CompareTo(other.Y);
    }

    public static bool operator ==(BoardCell<TState> left, BoardCell<TState> right) => left.Equals(right);

    public static bool operator !=(BoardCell<TState> left, BoardCell<TState> right) => !(left == right);

    public static bool operator <(BoardCell<TState> left, BoardCell<TState> right) => left.CompareTo(right) < 0;

    public static bool operator <=(BoardCell<TState> left, BoardCell<TState> right) => left.CompareTo(right) <= 0;

    public static bool operator >(BoardCell<TState> left, BoardCell<TState> right) => left.CompareTo(right) > 0;

    public static bool operator >=(BoardCell<TState> left, BoardCell<TState> right) => left.CompareTo(right) >= 0;
}
