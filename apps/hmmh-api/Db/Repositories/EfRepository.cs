using Hmmh.Api.Db.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hmmh.Api.Db.Repositories;

/// <summary>
///     Generic EF Core repository implementation.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public sealed class EfRepository<T> : IRepository<T> where T : class
{
    private readonly HmmhDbContext dbContext;
    private readonly DbSet<T> entities;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EfRepository{T}" /> class.
    /// </summary>
    /// <param name="dbContext">Database context for repository operations.</param>
    public EfRepository(HmmhDbContext dbContext)
    {
        this.dbContext = dbContext;
        entities = dbContext.Set<T>();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        // Check for any matching entity.
        return await entities.AsNoTracking().AnyAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking,
        CancellationToken cancellationToken)
    {
        // Load a single entity matching the predicate.
        var query = tracking ? entities : entities.AsNoTracking();
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> ListAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking,
        Func<IQueryable<T>, IQueryable<T>>? queryShaper,
        CancellationToken cancellationToken)
    {
        // Load all entities matching the predicate and custom query shape.
        var query = tracking ? entities : entities.AsNoTracking();
        query = query.Where(predicate);

        if (queryShaper is not null)
        {
            query = queryShaper(query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        // Add the entity to the change tracker.
        await entities.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public void Remove(T entity)
    {
        // Remove the entity from the change tracker.
        entities.Remove(entity);
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        // Persist pending changes to the database.
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
