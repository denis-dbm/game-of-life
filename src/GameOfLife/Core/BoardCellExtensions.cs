using System.Runtime.CompilerServices;

namespace GameOfLife.Core;

/// <summary>
/// Contains extensions methods for the <see cref="BoardCell{T}"/> class.
/// </summary>
public static class BoardCellExtensions
{
    /// <summary>
    /// Visits the neighborhood of the cell and applies the visitor to each cell in the neighborhood.
    /// </summary>
    /// <typeparam name="TState">The inferred type of the cell's state - to allow any type of state</typeparam>
    /// <typeparam name="TNeighborhoodVisitor">The inferred concrete type of a visitor.</typeparam>
    /// <param name="cell">A <see cref="BoardCell{T}"/>.</param>
    /// <param name="neighborhoodVisitor">An instance of the visitor passed by reference to allow value types too.</param>
    /// <remarks>
    /// The method tells to the JIT to inline the code to improve performance avoiding jumps (jmp) since the number of interactions is small and predictable.
    /// The visitor interface is open via the <typeparamref name="TNeighborhoodVisitor"/> type parameter to allow value types to be used without boxing and virtual calls (callvirt).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void VisitNeighborhood<TState, TNeighborhoodVisitor>(this BoardCell<TState> cell, ref TNeighborhoodVisitor neighborhoodVisitor)
        where TNeighborhoodVisitor : INeighborhoodVisitor
    {
        neighborhoodVisitor.Visit(cell.X - 1, cell.Y - 1);
        neighborhoodVisitor.Visit(cell.X - 1, cell.Y);
        neighborhoodVisitor.Visit(cell.X - 1, cell.Y + 1);
        neighborhoodVisitor.Visit(cell.X, cell.Y - 1);
        neighborhoodVisitor.Visit(cell.X, cell.Y + 1);
        neighborhoodVisitor.Visit(cell.X + 1, cell.Y - 1);
        neighborhoodVisitor.Visit(cell.X + 1, cell.Y);
        neighborhoodVisitor.Visit(cell.X + 1, cell.Y + 1);
        neighborhoodVisitor.Done(cell.X, cell.Y);
    }
}
