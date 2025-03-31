using GameOfLife.Core;
using GameOfLife.Presentation.Conversions;
using FluentAssertions;
using GameOfLife.Presentation;
using static GameOfLife.Core.Liveness;

namespace GameOfLife.UnitTest.Presentation.Conversions;

public class BoardObjectConverterTest
{
    [Fact]
    public void TryInput_ValidBoardState_ReturnsTrueAndCreatesBoard()
    {
        // Arrange
        var converter = new BoardObjectConverter();
        var boardState = new BoardState(0, [ "1.", ".1" ]);
        
        // Act
        var result = converter.TryInput(boardState, out var board, out var errorMessage);

        // Assert
        result.Should().BeTrue();
        board.Should().NotBeNull();
        errorMessage.Should().BeNull();
        board.Generation.Should().Be(0);
        board.Population.Should().Be(2);
        board.Cells.Should().BeEquivalentTo(new HashSet<LivenessBoardCell>
        {
            new(0, 0, Alive),
            new(1, 1, Alive)
        });
    }

    [Fact]
    public void TryInput_InvalidGeneration_ReturnsFalseAndErrorMessage()
    {
        // Arrange
        var converter = new BoardObjectConverter();
        var boardState = new BoardState(-1, [ "1.", ".1" ]);

        // Act
        var result = converter.TryInput(boardState, out var board, out var errorMessage);

        // Assert
        result.Should().BeFalse();
        board.Should().BeNull();
        errorMessage.Should().Be("Generation must be greater than or equal to 0.");
    }

    [Fact]
    public void TryInput_InvalidMatrixCharacter_ReturnsFalseAndErrorMessage()
    {
        // Arrange
        var converter = new BoardObjectConverter();
        var boardState = new BoardState(0, [ "1.", ".X" ]);

        // Act
        var result = converter.TryInput(boardState, out var board, out var errorMessage);

        // Assert
        result.Should().BeFalse();
        board.Should().BeNull();
        errorMessage.Should().Be("Invalid character 'X' (1, 1) found in matrix.");
    }

    [Fact]
    public void View_ValidBoard_ReturnsReadOnlyBoardState()
    {
        // Arrange
        var converter = new BoardObjectConverter();
        var cells = new HashSet<LivenessBoardCell>
        {
            new LivenessBoardCell(0, 0, Alive),
            new LivenessBoardCell(1, 1, Alive)
        };
        var board = new Board(BoardId.New(), 1, cells);

        // Act
        var readOnlyBoardState = converter.View(board);

        // Assert
        readOnlyBoardState.Generation.Should().Be(1);
        readOnlyBoardState.Cells.Should().BeEquivalentTo(["....", ".1..", "..1.", "...."]); // there is padding cells
        readOnlyBoardState.Population.Should().Be(2);
    }
}
