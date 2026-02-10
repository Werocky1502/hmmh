using Hmmh.Api.Db.Repositories;
using System.Linq.Expressions;

namespace Hmmh.Api.Tests.Services;

/// <summary>
///     In-memory repository used for service tests.
/// </summary>
internal sealed class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly List<T> items = new();

    /// <summary>
    ///     Gets the stored items for inspection in tests.
    /// </summary>
    public IReadOnlyList<T> Items => items;

    /// <inheritdoc />
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        // Evaluate the predicate against stored items.
        var exists = items.AsQueryable().Any(predicate);
        return Task.FromResult(exists);
    }

    /// <inheritdoc />
    public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, bool tracking, CancellationToken cancellationToken)
    {
        // Find the first item matching the predicate.
        var match = items.AsQueryable().FirstOrDefault(predicate);
        return Task.FromResult(match);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking,
        Func<IQueryable<T>, IQueryable<T>>? queryShaper,
        CancellationToken cancellationToken)
    {
        // Build the query based on the predicate and optional shaper.
        var query = items.AsQueryable().Where(predicate);
        if (queryShaper is not null)
        {
            query = queryShaper(query);
        }

        return Task.FromResult<IReadOnlyList<T>>(query.ToList());
    }

    /// <inheritdoc />
    public Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        // Add the entity to the list.
        items.Add(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove(T entity)
    {
        // Remove the entity from the list.
        items.Remove(entity);
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        // No-op for in-memory repository.
        return Task.CompletedTask;
    }
}
