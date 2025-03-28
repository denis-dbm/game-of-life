using GameOfLife.Core;

namespace GameOfLife.Persistence;

public interface IBoardRepository
{
    Task Add(Board board, CancellationToken cancellationToken);

    Task<Board?> Get(BoardId boardId, CancellationToken cancellationToken);

    Task<bool> Update(Board board, CancellationToken cancellationToken);

    Task<bool> Remove(BoardId boardId, CancellationToken cancellationToken);
}
