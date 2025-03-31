using GameOfLife.Core;
using MongoDB.Driver;

namespace GameOfLife.Persistence;

public class MongoBoardRepository(IMongoCollection<Board> collection) : IBoardRepository
{
    public Task Add(Board board, CancellationToken cancellationToken) =>
        collection.InsertOneAsync(board, cancellationToken: cancellationToken);

    public async Task<Board?> Get(BoardId boardId, CancellationToken cancellationToken) => 
        await collection.Find(x => x.Id == boardId).FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> Remove(BoardId boardId, CancellationToken cancellationToken)
    {
        var result = await collection.DeleteOneAsync(p => p.Id == boardId, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> Update(Board board, long originalGeneration, CancellationToken cancellationToken)
    {
        var result = await collection.ReplaceOneAsync(x => x.Id == board.Id && x.Generation == originalGeneration, board, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}
