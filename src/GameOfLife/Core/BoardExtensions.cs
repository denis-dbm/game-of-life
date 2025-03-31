namespace GameOfLife.Core;

/// <summary>
/// Contains extensions methods for the <see cref="Board"/> class.
/// </summary>
public static class BoardExtensions
{
    public static bool TryRunGenerations(this Board board, long generations, bool expectFinalState, out Board ranBoard)
    {
        if (generations < 1)
            throw new ArgumentOutOfRangeException(nameof(generations), "Number of generations to run must be greater than 0.");

        int i;
        ranBoard = new Board(board.Id, board.Generation, board.Cells);

        for (i = 0; i < generations; i++)
        {
            _ = ranBoard.NextGeneration();

            if (!ranBoard.HasMutatedFromLastGeneration)
                break;
        }

        long currentGeneration = ranBoard.Generation;
        bool prematureEnd = i < generations - 1;
        bool isUnfinished = expectFinalState && ranBoard.HasMutatedFromLastGeneration && ranBoard.NextGeneration(freezeOnNonMutation: true) != currentGeneration;

        if (isUnfinished)
            ranBoard = board;
        else if (prematureEnd && !ranBoard.HasMutatedFromLastGeneration)
            ranBoard.SetGeneration(currentGeneration + (generations - (i + 1)));

        return ranBoard != board;
    }
}
