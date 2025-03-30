using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using GameOfLife.Core;

namespace GameOfLife.Persistence.Mapping;

public class BoardIdSerializer : StructSerializerBase<BoardId>, IEnableableConfiguration
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BoardId value)
    {
        context.Writer.WriteString(value.Value);
    }

    public override BoardId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        string? value = context.Reader.ReadString();
        
        if (value is null || !BoardId.TryParse(value, out var boardId))
            throw new InvalidOperationException($"Invalid state for BoardId: {value}");
        
        return boardId;
    }

    public static void Enable()
    {
        BsonSerializer.RegisterSerializer(new BoardIdSerializer());
    }
}