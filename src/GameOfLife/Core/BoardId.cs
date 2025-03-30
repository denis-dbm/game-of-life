using static NanoidDotNet.Nanoid;

namespace GameOfLife.Core;

/// <summary>
/// Implements a value type to strongly-typed identify a <see cref="Board"/> object.
/// </summary>
public readonly struct BoardId
{
    private const int StringLength = 15;
    private const string Alphabet = Alphabets.Default;

    private static readonly HashSet<char> _alphabet = [.. Alphabet];

    public static readonly BoardId None = default;

    public string? Value { get; }

    private BoardId(string value) => Value = value;

    public static BoardId New() => new(Generate(Alphabet, size: StringLength));

    public static bool TryParse(string value, out BoardId boardId)
    {
        if (value.Length != StringLength || value.Any(c => !_alphabet.Contains(c)))
        {
            boardId = None;
            return false;
        }

        boardId = new BoardId(value);
        return true;
    }

    public override bool Equals(object? obj) => obj is BoardId boardId && Value == boardId.Value;

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString() => Value ?? string.Empty;

    public static bool operator ==(BoardId left, BoardId right) => left.Equals(right);

    public static bool operator !=(BoardId left, BoardId right) => !(left == right);
}
