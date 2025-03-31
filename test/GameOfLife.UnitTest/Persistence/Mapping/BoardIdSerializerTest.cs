using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using FluentAssertions;
using GameOfLife.Core;
using GameOfLife.Persistence.Mapping;

namespace GameOfLife.UnitTest.Persistence.Mapping;

public class BoardIdSerializerTest
{
    [Fact]
    public void Serialize_ShouldWriteStringValue()
    {
        // Arrange
        var boardId = BoardId.New();
        var serializer = new BoardIdSerializer();
        var writer = new BsonDocumentWriter(new BsonDocument());
        var context = BsonSerializationContext.CreateRoot(writer);
        context.Writer.WriteStartDocument();
        context.Writer.WriteName("$value");

        // Act
        serializer.Serialize(context, default, boardId);

        // Assert
        writer.Document.TryGetValue("$value", out var value);
        value.Should().Be(boardId.Value);
    }

    [Fact]
    public void Deserialize_ShouldReturnValidBoardId()
    {
        // Arrange
        var boardId = BoardId.New();
        var bsonReader = new BsonDocumentReader(new BsonDocument { { "$value", boardId.Value } });
        var context = BsonDeserializationContext.CreateRoot(bsonReader);
        var serializer = new BoardIdSerializer();
        context.Reader.ReadStartDocument();
        context.Reader.ReadName("$value");

        // Act
        var result = serializer.Deserialize(context, args: default);

        // Assert
        result.Value.Should().Be(boardId.Value);
    }

    [Fact]
    public void Deserialize_InvalidValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var bsonReader = new BsonDocumentReader(new BsonDocument { { "$value", "invalid-id" } });
        var context = BsonDeserializationContext.CreateRoot(bsonReader);
        var serializer = new BoardIdSerializer();
        context.Reader.ReadStartDocument();
        context.Reader.ReadName("$value");

        // Act & Assert
        Func<BoardId> action = () => serializer.Deserialize(context, args: default);
        action.Should().Throw<InvalidOperationException>().WithMessage("Invalid state for BoardId: invalid-id");
    }
}