using GameOfLife.Core;
using GameOfLife.Persistence.Mapping;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GameOfLife.Persistence;

/// <summary>
/// A module for setting up MongoDB persistence and the repositories.
/// </summary>
public static class MongoPersistenceModule
{
    public static IServiceCollection AddMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Mongo");

        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase("gameOfLifeDb");

        BoardIdSerializer.Enable();
        LowerCaseElementNameConvention.Enable();
        LoadSerializers();
        
        AddRepository<Board, IBoardRepository, MongoBoardRepository>(services, database, collectionName: "boards");

        return services;

        static IServiceCollection AddRepository<TCollection, TRepository, TRepositoryImpl>(IServiceCollection services, IMongoDatabase database, string collectionName)
            where TRepository : class
            where TRepositoryImpl : class, TRepository
            where TCollection : class
        {
            services.AddSingleton(database.GetCollection<TCollection>(collectionName));
            return services.AddSingleton<TRepository, TRepositoryImpl>();
        }
    }

    /// <summary>
    /// Setups by convention the needed serializers for the MongoDB persistence.
    /// This implementation generates fake writes to ensure registration of the serializers for "complex" (i.e. have a set) types.
    /// </summary>
    private static void LoadSerializers()
    {
        var writer = new BsonBinaryWriter(Stream.Null);
        BsonSerializer.Serialize(writer, new Board(BoardId.New(), 0, new HashSet<LivenessBoardCell>()));
    }
}
