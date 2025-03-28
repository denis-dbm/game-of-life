using GameOfLife.Core;
using MongoDB.Driver;

namespace GameOfLife.Persistence;

public class MongoDbBoardRepository : IBoardRepository
{
    private readonly IMongoCollection<Board> _collection;

    public MongoDbBoardRepository(MongoClient client)
    {
        var database = client.GetDatabase("GameOfLife");
        _collection = database.GetCollection<Board>("Board");
    }

    public Task Add(Board board, CancellationToken cancellationToken) =>
        _collection.InsertOneAsync(board, cancellationToken: cancellationToken);

    public async Task<Board?> Get(BoardId boardId, CancellationToken cancellationToken) => 
        await _collection.Find(x => x.Id == boardId).FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> Remove(BoardId boardId, CancellationToken cancellationToken)
    {
        var result = await _collection.DeleteOneAsync(p => p.Id == boardId, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> Update(Board board, CancellationToken cancellationToken)
    {
        var result = await _collection.ReplaceOneAsync(x => x.Id == board.Id, board, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}
