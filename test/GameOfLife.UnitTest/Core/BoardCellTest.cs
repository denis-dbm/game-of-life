using FluentAssertions;
using GameOfLife.Core;

namespace GameOfLife.UnitTest.Core;

public class BoardCellTest
{
    [Fact]
    public void BoardCell_Equality_ShouldBeTrueForSameCoordinates()
    {
        // Arrange & Act
        var cell1 = new BoardCell<int>(1, 1, 5);
        var cell2 = new BoardCell<int>(1, 1, 10);

        // Assert
        cell1.Should().Be(cell2);
        (cell1 == cell2).Should().BeTrue();
    }

    [Fact]
    public void BoardCell_Equality_ShouldBeFalseForDifferentCoordinates()
    {
        // Arrange & Act
        var cell1 = new BoardCell<int>(1, 1, 5);
        var cell2 = new BoardCell<int>(2, 1, 5);

        cell1.Should().NotBe(cell2);
        (cell1 != cell2).Should().BeTrue();
    }

    [Fact]
    public void BoardCell_Comparison_ShouldCompareCorrectly()
    {
        // Arrange & Act
        var cell1 = new BoardCell<int>(1, 1, 5);
        var cell2 = new BoardCell<int>(2, 1, 5);

        // Assert
        (cell1 < cell2).Should().BeTrue();
        (cell1 <= cell2).Should().BeTrue();
        (cell2 > cell1).Should().BeTrue();
        (cell2 >= cell1).Should().BeTrue();
    }

    [Fact]
    public void BoardCell_HashCode_ShouldBeEqualForSameCoordinates()
    {
        // Arrange & Act
        var cell1 = new BoardCell<int>(1, 1, 5);
        var cell2 = new BoardCell<int>(1, 1, 10);

        // Assert
        cell1.GetHashCode().Should().Be(cell2.GetHashCode());
    }

    [Fact]
    public void BoardCell_State_ShouldBeAssignable()
    {
        // Arrange & Act
        var cell = new BoardCell<string>(1, 1, "Alive");

        // Assert
        cell.State.Should().Be("Alive");
    }
}
