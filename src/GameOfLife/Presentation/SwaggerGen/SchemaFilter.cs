using GameOfLife.Core;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameOfLife.Presentation.SwaggerGen;

/// <summary>
/// A filter to modify or adjust the OpenAPI parameters schema.
/// </summary>
public class SchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(BoardId))
            schema.Type = "string";
    }
}
