namespace GameOfLife.Presentation;

/// <summary>
/// Represents a request to create a new board.
/// </summary>
/// <param name="Board">The initial board state.</param>
public record CreateBoardRequest(BoardState Board);

/// <summary>
/// Represents a request to run the next state of the board.
/// </summary>
/// <param name="Generations">The number of generations to run over the given board. Should be > 0.</param>
/// <param name="DryRun">Whether runs the board generation but does not save the processed state.</param>
/// <param name="ExpectFinalState">Whether the running board should reach the final state; otherwise an error is returned.</param>
public record RunNextBoardStateRequest(long Generations, bool DryRun, bool ExpectFinalState);
/// <summary>
/// Represents a response containing the next state of the board.
/// </summary>
/// <param name="Board">The board state.</param>
public record RunNextBoardStateResponse(BoardState Board);

/// <summary>
/// Represents the board state.
/// </summary>
/// <param name="Generation">The number of the generation where the board is.</param>
/// <param name="Cells">A array of strings to represent the board state according to the Game of Life rules.</param>
public record BoardState(long Generation, IEnumerable<string> Cells);