using GameOfLife.Core;
using GameOfLife.Persistence;
using Microsoft.AspNetCore.Mvc;
using GameOfLife.Presentation.Conversions;
using static Microsoft.AspNetCore.Http.Results;

namespace GameOfLife.Presentation;

public static class GameOfLifeEndpoints
{
    public static async Task<IResult> Create(
        [FromServices] IBoardRepository boardRepository,
        [FromServices] IInputConverter<BoardState, Board> inputConverter,
        [FromBody] CreateBoardRequest request,
        CancellationToken cancellationToken)
    {
        if (!inputConverter.TryInput(request.Board, out var board, out var errorMessage))
            return BadRequest(errorMessage);
        
        await boardRepository.Add(board, cancellationToken);
        return CreatedAtRoute(nameof(Get), new { boardId = board.Id });
    }

    public static async Task<IResult> Get(
        [FromServices] IBoardRepository boardRepository,
        [FromServices] IOutputConverter<Board, BoardState> outputConverter,
        [FromRoute] BoardId boardId,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.Get(boardId, cancellationToken);

        if (board is null)
            return NotFound();

        return Ok(outputConverter.View(board));
    }

    public static async Task<IResult> RunNextState(
        [FromServices] IBoardRepository boardRepository,
        [FromServices] IOutputConverter<Board, BoardState> outputConverter,
        [FromRoute] BoardId boardId,
        [FromBody] RunNextBoardStateRequest request,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.Get(boardId, cancellationToken);

        if (board is null)
            return NotFound();

        if (!board.TryRunGenerations(request.Generations, request.ExpectFinalState, out board))
            return Text(statusCode: 412, content: $"The board does not meet the required state after {request.Generations} generation(s).");

        if (!request.DryRun && !await boardRepository.Update(board, cancellationToken))
            return Conflict("The board was not found in the database. So, the ran state has been discarded.");

        return Ok(new RunNextBoardStateResponse(outputConverter.View(board)));
    }

    public static async Task<IResult> Delete(
        [FromServices] IBoardRepository boardRepository,
        [FromRoute] BoardId boardId,
        CancellationToken cancellationToken)
    {
        var deleted = await boardRepository.Remove(boardId, cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    public static void MapBoardEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/boards", Create);
        endpoints.MapGet("/boards/{boardId}", Get);
        endpoints.MapPost("/boards/{boardId}/next-state", RunNextState);
        endpoints.MapDelete("/boards/{boardId}", Delete);
    }
}