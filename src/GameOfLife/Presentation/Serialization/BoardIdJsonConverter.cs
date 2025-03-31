using System.Text.Json;
using System.Text.Json.Serialization;
using GameOfLife.Core;

namespace GameOfLife.Presentation.Serialization;

/// <summary>
/// A converter for serializing and deserializing <see cref="BoardId"/>.
/// </summary>
public class BoardIdJsonConverter : JsonConverter<BoardId>
{
    public override BoardId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();

        if (value is null || !BoardId.TryParse(value, out var boardId))
            throw new JsonException($"Invalid BoardId format: {value}");

        return boardId;
    }

    public override void Write(Utf8JsonWriter writer, BoardId value, JsonSerializerOptions options) => 
        writer.WriteStringValue(value.Value);
}
