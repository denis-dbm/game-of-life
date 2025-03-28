using GameOfLife.Core;
using GameOfLife.Persistence;
using GameOfLife.Presentation;
using GameOfLife.Presentation.Conversions;

namespace GameOfLife;

public static class AppDependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IBoardRepository, MongoDbBoardRepository>();
        services.AddSingleton<IInputConverter<BoardState, Board>, BoardObjectConverter>();
        services.AddSingleton<IOutputConverter<Board, BoardState>, BoardObjectConverter>();
        services.AddSingleton<BoardMatrixConverter>();
        return services;
    }
}
