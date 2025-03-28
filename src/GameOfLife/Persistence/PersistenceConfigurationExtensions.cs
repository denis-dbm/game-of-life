using MongoDB.Driver;

namespace GameOfLife.Persistence;

public static class PersistenceConfigurationExtensions
{
    public static MongoClient CreateMongoClient(this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Storage:MongoDb:ConnectionString");
        return new MongoClient(connectionString);
    }
}
