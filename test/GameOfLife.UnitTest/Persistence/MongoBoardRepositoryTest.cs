using FluentAssertions;
using GameOfLife.Core;
using GameOfLife.Persistence;
using MongoDB.Driver;
using NSubstitute;

namespace GameOfLife.UnitTest.Persistence;

public class MongoBoardRepositoryTest
{
    [Fact]
    public async Task Add_ShouldInsertBoard()
    {
        // Arrange
        var mockCollection = Substitute.For<IMongoCollection<Board>>();
        var repository = new MongoBoardRepository(mockCollection);
        var board = new Board(BoardId.New(), 0, new HashSet<LivenessBoardCell>());
        var cancellationToken = CancellationToken.None;

        // Act
        await repository.Add(board, cancellationToken);

        // Assert
        await mockCollection.Received(1).InsertOneAsync(board, null, cancellationToken);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task Remove_ShouldReturnTrue_WhenBoardIsDeleted(int deletedCount)
    {
        // Arrange
        var mockCollection = Substitute.For<IMongoCollection<Board>>();
        var deleteResult = Substitute.For<DeleteResult>();
        deleteResult.DeletedCount.Returns(deletedCount);

        mockCollection.DeleteOneAsync(Arg.Any<FilterDefinition<Board>>(), Arg.Any<CancellationToken>())
            .Returns(deleteResult);

        var repository = new MongoBoardRepository(mockCollection);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await repository.Remove(BoardId.New(), cancellationToken);

        // Assert
        result.Should().Be(deletedCount > 0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task Update_ShouldReturnTrue_WhenBoardIsUpdated(int modifiedCount)
    {
        // Arrange
        var mockCollection = Substitute.For<IMongoCollection<Board>>();
        var replaceResult = Substitute.For<ReplaceOneResult>();
        replaceResult.ModifiedCount.Returns(modifiedCount);

        mockCollection.ReplaceOneAsync(
            Arg.Any<FilterDefinition<Board>>(),
            Arg.Any<Board>(),
            default(ReplaceOptions?),
            Arg.Any<CancellationToken>())
            .Returns(replaceResult);

        var repository = new MongoBoardRepository(mockCollection);
        var board = new Board(BoardId.New(), 0, new HashSet<LivenessBoardCell>());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await repository.Update(board, board.Generation, cancellationToken);

        // Assert
        result.Should().Be(modifiedCount > 0);
    }
}
