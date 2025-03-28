namespace GameOfLife.Presentation;

public record CreateBoardRequest(BoardState Board);

public record RunNextBoardStateRequest(long Generations, bool DryRun, bool ExpectFinalState);
public record RunNextBoardStateResponse(BoardState Board);

public record BoardState(long Generation, IEnumerable<string> Cells);