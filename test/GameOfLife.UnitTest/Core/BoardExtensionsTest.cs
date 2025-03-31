using FluentAssertions;
using GameOfLife.Core;
using static GameOfLife.Core.Liveness;

namespace GameOfLife.UnitTest.Core;

public class BoardExtensionsTest
{
    [Fact]
    public void TryRunGenerations_ShouldThrowArgumentOutOfRangeException_WhenGenerationsIsLessThanOne()
    {
        // Arrange
        var board = new Board(BoardId.New(), 0, new HashSet<LivenessBoardCell>());

        // Act
        Action act = () => board.TryRunGenerations(0, false, out _);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void TryRunGenerations_ShouldRunGenerations_WhenNoFinalStateIsExpectedForBlinkerPattern()
    {
        // Arrange
        HashSet<LivenessBoardCell> cells = [
            new LivenessBoardCell(0, 0, Alive),
            new LivenessBoardCell(0, 1, Alive),
            new LivenessBoardCell(0, 2, Alive)
        ];
        var board = new Board(BoardId.New(), 0, cells);

        // Act
        var result = board.TryRunGenerations(generations: 15, expectFinalState: false, out var ranBoard);

        // Assert
        result.Should().BeTrue();
        ranBoard.Should().NotBeSameAs(board);
        ranBoard.Generation.Should().Be(15);
        ranBoard.HasMutatedFromLastGeneration.Should().BeTrue();
        ranBoard.Population.Should().Be(cells.Count);
    }

    [Fact]
    public void TryRunGenerations_ShouldFailToRunGenerations_WhenFinalStateIsExpectedForBlinkPattern()
    {
        // Arrange
        HashSet<LivenessBoardCell> cells = [
            new LivenessBoardCell(0, 0, Alive),
            new LivenessBoardCell(0, 1, Alive),
            new LivenessBoardCell(0, 2, Alive)
        ];
        var board = new Board(BoardId.New(), 0, cells);

        // Act
        var result = board.TryRunGenerations(generations: 15, expectFinalState: true, out var ranBoard);

        // Assert
        result.Should().BeFalse();
        ranBoard.Should().BeSameAs(board);
        ranBoard.Generation.Should().Be(0);
        ranBoard.HasMutatedFromLastGeneration.Should().BeFalse();
        ranBoard.Population.Should().Be(cells.Count);
    }
}
