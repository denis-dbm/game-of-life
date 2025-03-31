using System.Diagnostics.CodeAnalysis;
using System.Text;
using GameOfLife.Core;

namespace GameOfLife.Presentation.Conversions;

/// <summary>
/// Implements the conversion between the <see cref="BoardState" /> (and variants like <see cref="ReadOnlyBoardState" />) and <see cref="Board" /> models.
/// </summary>
public class BoardObjectConverter : IInputConverter<BoardState, Board>, IOutputConverter<Board, ReadOnlyBoardState>
{
    private readonly BoardMatrixConverter _boardMatrixConverter = new();

    public bool TryInput(BoardState presentation, [NotNullWhen(true)] out Board? model, [NotNullWhen(false)] out string? errorMessage)
    {
        if (!_boardMatrixConverter.TryInput(presentation.Cells, out var cells, out errorMessage))
        {
            model = null;
            return false;
        }

        if (presentation.Generation < 0)
        {
            errorMessage = "Generation must be greater than or equal to 0.";
            model = null;
            return false;
        }

        model = new Board(BoardId.New(), presentation.Generation, cells);
        return true;
    }

    public ReadOnlyBoardState View(Board model) => new(model.Generation, _boardMatrixConverter.View(model.Cells), model.Population);

    private sealed class BoardMatrixConverter :
        IInputConverter<IEnumerable<string>, HashSet<LivenessBoardCell>>,
        IOutputConverter<IReadOnlySet<LivenessBoardCell>, IEnumerable<string>>
    {
        private const char DeadOrEmpty = '.';
        private const char Alive = '1';

        public bool TryInput(IEnumerable<string> matrix, [NotNullWhen(true)] out HashSet<LivenessBoardCell>? cells, [NotNullWhen(false)] out string? errorMessage)
        {
            _ = matrix.TryGetNonEnumeratedCount(out var count);
            cells = new HashSet<LivenessBoardCell>(count);
            long x = 0;

            foreach (var line in matrix)
            {
                long y = 0;

                foreach (var cell in line)
                    switch (cell)
                    {
                        case DeadOrEmpty:
                            y++;
                            continue;
                        case Alive:
                            cells.Add(new(x, y++, Liveness.Alive));
                            break;
                        default:
                            cells = null;
                            errorMessage = $"Invalid character '{cell}' ({x}, {y}) found in matrix.";
                            return false;
                    }

                x++;
            }

            errorMessage = null;
            return true;
        }

        public IEnumerable<string> View(IReadOnlySet<LivenessBoardCell> model)
        {
            long minX = long.MaxValue, maxX = long.MinValue;
            long minY = long.MaxValue, maxY = long.MinValue;
            var line = new StringBuilder();

            foreach (var cell in model)
            {
                if (cell.X < minX) minX = cell.X;
                if (cell.X > maxX) maxX = cell.X;
                if (cell.Y < minY) minY = cell.Y;
                if (cell.Y > maxY) maxY = cell.Y;
            }

            for (long i = minX - 1; i <= maxX + 1; i++)
            {
                for (long j = minY - 1; j <= maxY + 1; j++)
                {
                    var cell = new LivenessBoardCell(i, j);
                    line.Append(model.Contains(cell) ? Alive : DeadOrEmpty);
                }

                yield return line.ToString();
                line.Clear();
            }
        }
    }
}