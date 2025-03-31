using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GameOfLife.Core;
using GameOfLife.Persistence;
using GameOfLife.Presentation;
using GameOfLife.Presentation.Conversions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Xunit;

namespace GameOfLife.UnitTest.Presentation;

public class BoardEndpointsTest
{
    [Fact]
    public async Task Create_ShouldReturnCreatedAtRoute_WhenBoardIsValid()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockInputConverter = Substitute.For<IInputConverter<BoardState, Board>>();
        var request = new CreateBoardRequest(new BoardState(0, []));
        var board = new Board(BoardId.New(), 0, new HashSet<LivenessBoardCell>());

        mockInputConverter
            .TryInput(request.Board, out _, out _)
            .Returns(x =>
            {
                x[1] = board;
                return true;
            });

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Create(mockBoardRepository, mockInputConverter, request, cancellationToken);
        
        // Assert
        result.Result.Should().BeOfType<CreatedAtRoute>();
        result.Result.As<CreatedAtRoute>().RouteName.Should().Be("Get");
        result.Result.As<CreatedAtRoute>().RouteValues["boardId"].Should().Be(board.Id);
        await mockBoardRepository.Received(1).Add(board, cancellationToken);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenBoardIsInvalid()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockInputConverter = Substitute.For<IInputConverter<BoardState, Board>>();
        var request = new CreateBoardRequest(new BoardState(0, []));

        mockInputConverter
            .TryInput(request.Board, out _, out _)
            .Returns(x =>
            {
                x[2] = "Invalid board";
                return false;
            });

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Create(mockBoardRepository, mockInputConverter, request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<BadRequest<string>>();
        result.Result.As<BadRequest<string>>().Value.Should().Be("Invalid board");
    }

    [Fact]
    public async Task Get_ShouldReturnOk_WhenBoardExists()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();
        var board = new Board(boardId, generation: 9, new HashSet<LivenessBoardCell>() 
        {
            new(0, 0, Liveness.Alive),
            new(0, 1, Liveness.Alive),
            new(0, 2, Liveness.Alive),
        });
        var readOnlyBoardState = new ReadOnlyBoardState(Generation: 9, Cells: [".....", ".111.", "....."], Population: 3);

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(board);
        mockOutputConverter.View(board).Returns(readOnlyBoardState);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Get(mockBoardRepository, mockOutputConverter, boardId, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<Ok<ReadOnlyBoardState>>();
        result.Result.As<Ok<ReadOnlyBoardState>>().Value.Should().Be(readOnlyBoardState);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(default(Board?));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Get(mockBoardRepository, mockOutputConverter, boardId, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task RunNextState_ShouldReturnOk_WhenBoardExistsAndStateIsUpdated()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();
        var board = new Board(boardId, generation: 5, new HashSet<LivenessBoardCell>());
        var request = new RunNextBoardStateRequest(Generations: 1, ExpectFinalState: true, DryRun: false);
        var readOnlyBoardState = new ReadOnlyBoardState(Generation: 6, Cells: [], Population: 0);

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(board);
        mockBoardRepository.Update(Arg.Any<Board>(), originalGeneration: 5, Arg.Any<CancellationToken>()).Returns(true);
        mockOutputConverter.View(Arg.Any<Board>()).Returns(readOnlyBoardState);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.RunNextState(mockBoardRepository, mockOutputConverter, boardId, request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<Ok<RunNextBoardStateResponse>>();
        result.Result.As<Ok<RunNextBoardStateResponse>>().Value!.Board.Should().Be(readOnlyBoardState);
    }

    [Fact]
    public async Task RunNextState_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();
        var request = new RunNextBoardStateRequest(Generations: 1, ExpectFinalState: true, DryRun: false);

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(default(Board?));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.RunNextState(mockBoardRepository, mockOutputConverter, boardId, request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task RunNextState_ShouldReturnConflict_WhenBoardUpdateFails()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();
        var board = new Board(boardId, generation: 5, new HashSet<LivenessBoardCell>());
        var request = new RunNextBoardStateRequest(Generations: 1, ExpectFinalState: true, DryRun: false);

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(board);
        _ = board.TryRunGenerations(request.Generations, request.ExpectFinalState, out var updatedBoard);
        mockBoardRepository.Update(updatedBoard, board.Generation, Arg.Any<CancellationToken>()).Returns(false);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.RunNextState(mockBoardRepository, mockOutputConverter, boardId, request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<Conflict<string>>();
        result.Result.As<Conflict<string>>().Value.Should().Be("The board was not found in the database or it was updated by other request. So, the ran state has been discarded.");
    }

    [Fact]
    public async Task RunNextState_ShouldReturnPreconditionFailed_WhenBoardDoesNotMeetRequiredState()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var mockOutputConverter = Substitute.For<IOutputConverter<Board, ReadOnlyBoardState>>();
        var boardId = BoardId.New();
        var board = new Board(boardId, generation: 5, new HashSet<LivenessBoardCell>() {
            new(0, 0, Liveness.Alive),
            new(0, 1, Liveness.Alive),
            new(0, 2, Liveness.Alive)
        });
        var request = new RunNextBoardStateRequest(Generations: 1, ExpectFinalState: true, DryRun: false);

        mockBoardRepository.Get(boardId, Arg.Any<CancellationToken>()).Returns(board);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.RunNextState(mockBoardRepository, mockOutputConverter, boardId, request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<ContentHttpResult>();
        result.Result.As<ContentHttpResult>().StatusCode.Should().Be(412);
        result.Result.As<ContentHttpResult>().ResponseContent.Should().Be("The board does not meet the required state after 1 generation(s).");
    }


    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenBoardIsDeleted()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var boardId = BoardId.New();

        mockBoardRepository.Remove(boardId, Arg.Any<CancellationToken>()).Returns(true);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Delete(mockBoardRepository, boardId, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenBoardDoesNotExist()
    {
        // Arrange
        var mockBoardRepository = Substitute.For<IBoardRepository>();
        var boardId = BoardId.New();

        mockBoardRepository.Remove(boardId, Arg.Any<CancellationToken>()).Returns(false);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await BoardEndpoints.Delete(mockBoardRepository, boardId, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public void MapBoardEndpoints_ShouldMapAllEndpoints()
    {
        // Arrange
        IEndpointRouteBuilder endpoints = WebApplication.CreateBuilder().Build();

        // Act
        BoardEndpoints.MapBoardEndpoints(endpoints);

        // Assert
        endpoints.DataSources.Should().HaveCount(1);
        endpoints.DataSources.First().Endpoints.Select(e => e.DisplayName).Should().BeEquivalentTo(
            "HTTP: POST /boards => Create",
            "HTTP: GET /boards/{boardId} => Get",
            "HTTP: POST /boards/{boardId}/next-state => RunNextState",
            "HTTP: DELETE /boards/{boardId} => Delete"
        );
    }
}
