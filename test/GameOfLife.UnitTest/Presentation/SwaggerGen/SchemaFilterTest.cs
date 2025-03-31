using FluentAssertions;
using GameOfLife.Core;
using GameOfLife.Presentation.SwaggerGen;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameOfLife.UnitTest.Presentation.SwaggerGen;

public class SchemaFilterTest
{
    [Theory]
    [InlineData(typeof(BoardId), "string")]
    [InlineData(typeof(int), null)]
    public void Apply_ShouldSetSchemaTypeToString_WhenTypeIs(Type type, string? mappedTo)
    {
        // Arrange
        var schema = new OpenApiSchema();
        var context = new SchemaFilterContext(
            type,
            schemaGenerator: null,
            schemaRepository: null
        );
        var schemaFilter = new SchemaFilter();

        // Act
        schemaFilter.Apply(schema, context);

        // Assert
        schema.Type.Should().Be(mappedTo);
    }
}
