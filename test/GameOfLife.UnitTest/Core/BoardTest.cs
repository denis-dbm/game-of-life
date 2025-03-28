namespace GameOfLife.UnitTest.Core;

using static GameOfLife.Core.Liveness;
using GameOfLife.Core;
using FluentAssertions;

public class BoardTest
{
    [Theory]
    [MemberData(nameof(SampleLoopingPatternsCellsState))]
    public void GivenStateAsInitialOrNot_ShouldBeAsExpected(ExpectedNextState expectedNextState)
    {
        // Arrange
        const long expectedGeneration = 1;
        var board = new Board(BoardId.New(), generation: 0, expectedNextState.Initial);

        // Act
        long generation = board.NextGeneration();

        // Assert
        generation.Should().Be(expectedGeneration);
        board.Generation.Should().Be(expectedGeneration);
        board.Cells().Should().BeEquivalentTo(expectedNextState.Next);
    }

    [Theory]
    [MemberData(nameof(SampleLoopingPatternsCellsState))]
    public void GivenLoopingPatterns_ShouldProcessTheLifeInfinitely(ExpectedNextState expectedNextState)
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
            board.Cells().Should().BeEquivalentTo(expectedNextState.Next);
            (expectedNextState.Initial, expectedNextState.Next) = (expectedNextState.Next, expectedNextState.Initial);
        }

        // Assert
        board.Generation.Should().Be(times);
    }

    [Fact]
    public void GivenGliderPattern_ShouldProcessTheLifeInfinitely()
    {
        // Arrange
        const int times = 100;
        HashSet<LivenessBoardCell> cells = [
            new LivenessBoardCell(0, 0, Alive),
            new LivenessBoardCell(1, 0, Alive),
            new LivenessBoardCell(2, 0, Alive),
            new LivenessBoardCell(0, 1, Alive),
            new LivenessBoardCell(1, 2, Alive)
        ];
        var board = new Board(BoardId.New(), generation: 0, cells);
        
        LivenessBoardCell[][] cellsFromMinPosition = [
            [
                new(1, 0, Alive),
                new(2, 0, Alive),
                new(0, 1, Alive),
                new(1, 2, Alive),
            ],
            [
                new(1, 0, Alive),
                new(0, 1, Alive),
                new(2, 1, Alive),
                new(1, -1, Alive),
            ],
            [
                new(1, 0, Alive),
                new(0, 1, Alive),
                new(0, 2, Alive),
                new(2, 1, Alive),
            ],
            [
                new(1, 0, Alive),
                new(2, 1, Alive),
                new(1, -1, Alive),
                new(1, -2, Alive),
            ],
            [
                new(1, 0, Alive),
                new(2, 0, Alive),
                new(0, -1, Alive),
                new(1, -2, Alive),
            ]
        ];

        for (int i = 0; i < times; i++)
        {
            // Act
            _ = board.NextGeneration();

            // Assert
            board.Cells().Count(c => c.State is Alive).Should().Be(5);

            // var minCell = board.Cells.Min();

            // foreach (var cell in cellsFromMinPosition)
            // {
            //     long x = minCell.X + cell.X;
            //     long y = minCell.Y;
            //     board.Cells.Contains(new LivenessBoardCell(x, y, Alive)).Should().BeTrue();
            // }
        }

        // Assert
        board.Generation.Should().Be(times);
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