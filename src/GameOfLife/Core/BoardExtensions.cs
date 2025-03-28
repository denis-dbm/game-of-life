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

        ranBoard = new Board(board.Id, board.Generation, board.Cells());

        for (int i = 0; i < generations - 1; i++)
            _ = ranBoard.NextGeneration();

        _ = ranBoard.NextGeneration();
        
        if (expectFinalState && ranBoard.HasMutatedFromLastGeneration)
        {
            ranBoard = board;
            return false;
        }

        return true;
    }
}
