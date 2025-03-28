namespace GameOfLife.Core;

/// <summary>
/// Supports operation to visit cells in a neighborhood, where a cell is identified by its coordinates.
/// </summary>
/// <seealso cref="BoardCell{T}"/>
public interface INeighborhoodVisitor
{
    void Visit(long x, long y);

    /// <summary>
    /// Signals when the visiting of the cells is done.
    /// </summary>
    /// <param name="originalX">The X pos. of the original cell that demanded the visit.</param>
    /// <param name="originalY">The Y pos. of the original cell that demanded the visit.</param>
    void Done(long originalX, long originalY);
}
