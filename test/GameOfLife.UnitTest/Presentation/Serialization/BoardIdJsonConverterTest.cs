using System.Text;
using System.Text.Json;
using FluentAssertions;
using GameOfLife.Core;
using GameOfLife.Presentation.Serialization;

namespace GameOfLife.UnitTest.Presentation.Serialization;

public class BoardIdJsonConverterTest
{
    [Fact]
    public void Read_ValidBoardId_ReturnsBoardId()
    {
        // Arrange
        var boardId = BoardId.New();
        ReadOnlySpan<byte> json = Encoding.UTF8.GetBytes($"\"{boardId.Value}\"");
        var converter = new BoardIdJsonConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };
        var reader = new Utf8JsonReader(json);

        // Act
        reader.Read(); // Move to the value
        var result = converter.Read(ref reader, typeof(BoardId), options);

        // Assert
        result.Should().Be(boardId);
    }

    [Fact]
    public void Read_InvalidBoardId_ThrowsJsonException()
    {
        // Arrange
        var json = "\"invalid-board-id\"";
        var converter = new BoardIdJsonConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

        // Act & Assert
        reader.Read(); // Move to the value

        try
        {
            converter.Read(ref reader, typeof(BoardId), options);
        }
        catch (JsonException ex)
        {
            ex.Message.Should().Contain("Invalid BoardId format");
        }
    }

    [Fact]
    public void Write_ValidBoardId_WritesCorrectJson()
    {
        // Arrange
        var boardId = BoardId.New();
        var converter = new BoardIdJsonConverter();
        var options = new JsonSerializerOptions { Converters = { converter } };
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        // Act
        converter.Write(writer, boardId, options);
        writer.Flush();
        var json = Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        json.Should().Be($"\"{boardId.Value}\"");
    }
}
