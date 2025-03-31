using GameOfLife.Core;
using GameOfLife.Persistence;
using GameOfLife.Presentation;
using GameOfLife.Presentation.Conversions;
using GameOfLife.Presentation.ErrorHandling;
using GameOfLife.Presentation.Serialization;
using MongoDB.Driver;

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
        CheckRequiredConfiguration(configuration);

        var boardObjectConverter = new BoardObjectConverter();

        services.AddMongoPersistence(configuration);
        services.AddSingleton<IInputConverter<BoardState, Board>>(boardObjectConverter);
        services.AddSingleton<IOutputConverter<Board, ReadOnlyBoardState>>(boardObjectConverter);
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new BoardIdJsonConverter());
        });

        return services;
    }

    private static void CheckRequiredConfiguration(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Mongo");
        
        try
        {
            _ = MongoUrl.Create(connectionString);
        }
        catch (Exception)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Invalid MongoDB connection string. Set a valid connection string (value={connectionString}). Checkout the README.md for more information.");
            Environment.Exit(1);
        }
    }
}
