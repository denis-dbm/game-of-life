using GameOfLife.Core;
using GameOfLife.Persistence;
using Microsoft.AspNetCore.Mvc;
using GameOfLife.Presentation.Conversions;
using static Microsoft.AspNetCore.Http.TypedResults;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameOfLife.Presentation;

/// <summary>
/// Implements the endpoints API for the Game of Life board.
/// </summary>
public static class BoardEndpoints
{
    /// <summary>
    /// Creates a new board with the given initial state.
    /// </summary>
    /// <remarks>
    /// Samples to represent the board cells' state:
    /// 
    ///      Block pattern:
    ///      001100
    ///      001100
    /// 
    ///      Blinker (horizontal) pattern:
    ///      111
    /// 
    ///      Blinker (vertical) pattern:
    ///      010
    ///      010
    ///      010
    /// 
    ///      Beacon pattern (missed 0s on first and second lines fallbacks to 0 - Dead):
    ///      11
    ///      11
    ///      0011
    ///      0011
    /// 
    /// The top-left corner of the board is represented by the first character of the first string at position (0, 0).
    /// Each line is a entry in the cells string array.
    /// </remarks>
    /// <param name="boardRepository"></param>
    /// <param name="inputConverter"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="201">The board was created successfully.</response>
    /// <response code="400">The request was invalid.</response>
    public static async Task<Results<CreatedAtRoute, BadRequest<string>>> Create(
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

    /// <summary>
    /// Retrieves the current state of the board.
    /// </summary>
    /// <param name="boardRepository"></param>
    /// <param name="outputConverter"></param>
    /// <param name="boardId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The board was found and returned.</response>
    /// <response code="400">Invalid format of ID was supplied.</response>
    /// <response code="404">The board was not found.</response>
    public static async Task<Results<Ok<ReadOnlyBoardState>, NotFound>> Get(
        [FromServices] IBoardRepository boardRepository,
        [FromServices] IOutputConverter<Board, ReadOnlyBoardState> outputConverter,
        [FromRoute] BoardId boardId,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.Get(boardId, cancellationToken);

        if (board is null)
            return NotFound();

        return Ok(outputConverter.View(board));
    }

    /// <summary>
    /// Runs the next state of the board and returns the result.
    /// </summary>
    /// <param name="boardRepository"></param>
    /// <param name="outputConverter"></param>
    /// <param name="boardId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The board was found and the next state was returned respecting the required state.</response>
    /// <response code="400">Invalid format of ID was supplied.</response>
    /// <response code="404">The board was not found.</response>
    /// <response code="409">The board was not found in the database after processing.</response>
    /// <response code="412">The board does not meet the required state after the specified generations.</response>
    public static async Task<Results<Ok<RunNextBoardStateResponse>, NotFound, Conflict<string>, ContentHttpResult>> RunNextState(
        [FromServices] IBoardRepository boardRepository,
        [FromServices] IOutputConverter<Board, ReadOnlyBoardState> outputConverter,
        [FromRoute] BoardId boardId,
        [FromBody] RunNextBoardStateRequest request,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.Get(boardId, cancellationToken);

        if (board is null)
            return NotFound();

        long originalGeneration = board.Generation;

        if (!board.TryRunGenerations(request.Generations, request.ExpectFinalState, out board))
            return Text(statusCode: 412, content: $"The board does not meet the required state after {request.Generations} generation(s).");

        if (!request.DryRun && !await boardRepository.Update(board, originalGeneration, cancellationToken))
            return Conflict("The board was not found in the database or it was updated by other request. So, the ran state has been discarded.");

        return Ok(new RunNextBoardStateResponse(outputConverter.View(board)));
    }

    /// <summary>
    /// Deletes the board with the given ID.
    /// </summary>
    /// <param name="boardRepository"></param>
    /// <param name="boardId"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">The board was deleted successfully.</response>
    /// <response code="400">Invalid format of ID was supplied.</response>
    /// <response code="404">The board was not found.</response>
    public static async Task<Results<NoContent, NotFound>> Delete(
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
        endpoints.MapGet("/boards/{boardId}", Get).WithName(nameof(Get));
        endpoints.MapPost("/boards/{boardId}/next-state", RunNextState);
        endpoints.MapDelete("/boards/{boardId}", Delete);
    }
}