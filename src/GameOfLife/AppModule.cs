using GameOfLife.Core;
using GameOfLife.Persistence;
using GameOfLife.Presentation;
using GameOfLife.Presentation.Conversions;
using GameOfLife.Presentation.Serialization;

namespace GameOfLife;

/// <summary>
/// This class is responsible for registering all the dependencies of the application.
/// This is the main module for dependency injection and configuration.
/// </summary>
/// <remarks>
/// A module is a logical grouping of related functionality that can be registered in the dependency injection container.
/// </remarks>
public static class AppModule
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoPersistence(configuration);
        services.AddSingleton<IInputConverter<BoardState, Board>, BoardObjectConverter>();
        services.AddSingleton<IOutputConverter<Board, BoardState>, BoardObjectConverter>();
        services.AddSingleton<BoardMatrixConverter>();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new BoardIdJsonConverter());
        });

        return services;
    }
}
