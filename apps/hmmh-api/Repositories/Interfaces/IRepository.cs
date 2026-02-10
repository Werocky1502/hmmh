using System.Linq.Expressions;

namespace Hmmh.Api.Repositories;

/// <summary>
///     Defines generic database operations for entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    ///     Checks whether any entity matches the predicate.
    /// </summary>
    /// <param name="predicate">Predicate for filtering.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>True when a match exists.</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    /// <summary>
    ///     Loads a single entity that matches the predicate.
    /// </summary>
    /// <param name="predicate">Predicate for filtering.</param>
    /// <param name="tracking">Whether to track the entity for updates.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The matching entity, or null when not found.</returns>
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, bool tracking, CancellationToken cancellationToken);

    /// <summary>
    ///     Loads entities that match the predicate.
    /// </summary>
    /// <param name="predicate">Predicate for filtering.</param>
    /// <param name="tracking">Whether to track the entities for updates.</param>
    /// <param name="queryShaper">Optional query customizations (ordering, includes).</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>List of matching entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking,
        Func<IQueryable<T>, IQueryable<T>>? queryShaper,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Adds a new entity to the database context.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes an entity from the database context.
    /// </summary>
    /// <param name="entity">Entity to remove.</param>
    void Remove(T entity);

    /// <summary>
    ///     Persists pending changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
