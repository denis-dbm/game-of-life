namespace GameOfLife.UnitTest.Core;

using static GameOfLife.Core.Liveness;
using GameOfLife.Core;
using FluentAssertions;

public class BoardTest
{
    [Theory]
    [MemberData(nameof(SampleLoopingPatternsCellsState))]
    public void NextGeneration_WithSampleLoopingPatterns_ShouldRunNextGeneration(ExpectedNextState expectedNextState)
    {
        // Arrange
        const long expectedGeneration = 1;
        var board = new Board(BoardId.New(), generation: 0, expectedNextState.Initial);

        // Act
        long generation = board.NextGeneration();

        // Assert
        generation.Should().Be(expectedGeneration);
        board.Generation.Should().Be(expectedGeneration);
        board.Cells.Should().BeEquivalentTo(expectedNextState.Next);
    }

    [Theory]
    [MemberData(nameof(SampleLoopingPatternsCellsState))]
    public void NextGeneration_WithSampleLoopingPatterns_ShouldProcessTheLifeInfinitely(ExpectedNextState expectedNextState)
    {
        // Arrange
        const int times = 50;
        long generation = 0;
        var board = new Board(BoardId.New(), generation, expectedNextState.Initial);

        for (int i = 0; i < times; i++)
        {
            // Act
            var newGeneration = board.NextGeneration();

            // Assert
            newGeneration.Should().Be(board.Generation);
            board.Generation.Should().Be(++generation);
            board.Cells.Should().BeEquivalentTo(expectedNextState.Next);
            (expectedNextState.Initial, expectedNextState.Next) = (expectedNextState.Next, expectedNextState.Initial);
        }

        // Assert
        board.Generation.Should().Be(times);
    }

    [Fact]
    public void SetGeneration_ShouldSetTheGeneration_WhenBoardDidNotMutate()
    {
        // Arrange
        const long expectedGeneration = 2;
        var board = new Board(BoardId.New(), generation: 0, new HashSet<LivenessBoardCell>());

        // Act
        board.NextGeneration();
        board.SetGeneration(board.Generation + 1);

        // Assert
        board.Generation.Should().Be(expectedGeneration);
    }

    [Fact]
    public void SetGeneration_ShouldNotSetTheGeneration_WhenBoardMutated()
    {
        // Arrange
        var board = new Board(BoardId.New(), generation: 0, new HashSet<LivenessBoardCell>()
        {
            new(0, 0, Alive),
            new(1, 0, Alive),
            new(2, 0, Alive)
        });

        // Act
        board.NextGeneration();
        Action act = () => board.SetGeneration(board.Generation + 1);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot set generation when the board has mutated from the last generation.");
    }

    public static IEnumerable<object[]> SampleLoopingPatternsCellsState()
    {
        // Blinker
        yield return [new ExpectedNextState()
        {
            Initial = [
                new(-1, 0, Alive),
                new(0, 0, Alive),
                new(1, 0, Alive)
            ],
            Next = [
                new(0, -1, Alive),
                new(0, 0, Alive),
                new(0, 1, Alive)
            ]
        }];

        // Toad
        yield return [new ExpectedNextState()
        {
            Initial = [
                new(0, 0, Alive),
                new(1, 0, Alive),
                new(2, 0, Alive),
                new(-1, 1, Alive),
                new(0, 1, Alive),
                new(1, 1, Alive)
            ],
            Next = [
                new(1, -1, Alive),
                new(-1, 0, Alive),
                new(2, 0, Alive),
                new(-1, 1, Alive),
                new(2, 1, Alive),
                new(0, 2, Alive)
            ]
        }];

        // Block
        yield return [new ExpectedNextState()
        {
            Initial = [
                new(0, 0, Alive),
                new(0, 1, Alive),
                new(1, 0, Alive),
                new(1, 1, Alive)
            ],
            Next = [
                new(0, 0, Alive),
                new(0, 1, Alive),
                new(1, 0, Alive),
                new(1, 1, Alive)
            ]
        }];

        // Tub
        yield return [new ExpectedNextState()
        {
            Initial = [
                new(0, 1, Alive),
                new(1, 0, Alive),
                new(0, -1, Alive),
                new(-1, 0, Alive)
            ],
            Next = [
                new(0, 1, Alive),
                new(1, 0, Alive),
                new(0, -1, Alive),
                new(-1, 0, Alive)
            ]
        }];

        // Beacon
        yield return [new ExpectedNextState()
        {
            Initial = [
                new(-1, -1, Alive),
                new(0, -1, Alive),
                new(-1, 0, Alive),
                new(0, 0, Alive),
                new(1, 1, Alive),
                new(2, 1, Alive),
                new(1, 2, Alive),
                new(2, 2, Alive)
            ],
            Next = [
                new(-1, -1, Alive),
                new(0, -1, Alive),
                new(-1, 0, Alive),
                new(2, 1, Alive),
                new(1, 2, Alive),
                new(2, 2, Alive)
            ]
        }];
    }

    public record struct ExpectedNextState(HashSet<LivenessBoardCell> Initial, HashSet<LivenessBoardCell> Next);
}