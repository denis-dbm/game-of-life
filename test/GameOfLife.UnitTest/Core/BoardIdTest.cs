
using FluentAssertions;
using GameOfLife.Core;

namespace GameOfLife.UnitTest.Core;

public class BoardIdTest
{
    [Fact]
    public void New_ShouldGenerateUniqueBoardId()
    {
        // Act
        var boardId1 = BoardId.New();
        var boardId2 = BoardId.New();

        // Assert
        boardId1.Value.Should().NotBeNull();
        boardId2.Value.Should().NotBeNull();
        boardId1.Value.Should().NotBe(boardId2.Value);
    }

    [Fact]
    public void TryParse_ValidValue_ShouldReturnTrueAndSetBoardId()
    {
        // Arrange
        var validValue = BoardId.New().Value;

        // Act
        var result = BoardId.TryParse(validValue!, out var boardId);

        // Assert
        result.Should().BeTrue();
        boardId.Value.Should().Be(validValue);
    }

    [Fact]
    public void TryParse_InvalidValue_ShouldReturnFalseAndSetBoardIdToNone()
    {
        // Arrange
        var invalidValue = "invalid_board_id";

        // Act
        var result = BoardId.TryParse(invalidValue, out var boardId);

        // Assert
        result.Should().BeFalse();
        boardId.Should().Be(BoardId.None);
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var value = BoardId.New().Value;
        _ = BoardId.TryParse(value!, out var boardId1);
        _ = BoardId.TryParse(value!, out var boardId2);

        // Act
        var result = boardId1.Equals(boardId2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var boardId1 = BoardId.New();
        var boardId2 = BoardId.New();

        // Act
        var result = boardId1.Equals(boardId2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var value = BoardId.New().Value;
        _ = BoardId.TryParse(value!, out var boardId1);
        _ = BoardId.TryParse(value!, out var boardId2);

        // Act
        var hashCode1 = boardId1.GetHashCode();
        var hashCode2 = boardId2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void BoardId_Equality_ShouldBeTrueForSameValues()
    {
        // Arrange & Act
        var boardId1 = BoardId.New();
        _ = BoardId.TryParse(boardId1.Value!, out var boardId2);

        // Assert
        (boardId1 == boardId2).Should().BeTrue();
    }

    [Fact]
    public void BoardId_Equality_ShouldBeFalseForDifferentValues()
    {
        // Arrange & Act
        var boardId1 = BoardId.New();
        var boardId2 = BoardId.New();

        // Assert
        (boardId1 != boardId2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var boardId = BoardId.New();
        var value = boardId.Value;

        // Act
        var result = boardId.ToString();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void None_ShouldHaveNullValue()
    {
        // Assert
        BoardId.None.Value.Should().BeNull();
    }
}
