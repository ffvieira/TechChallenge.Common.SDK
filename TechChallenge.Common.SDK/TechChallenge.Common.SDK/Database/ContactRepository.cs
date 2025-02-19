using Microsoft.EntityFrameworkCore;
using Polly;

namespace TechChallenge.Common.SDK.Database;
public class ContactRepository<TContext, TEntity> where TContext : DbContext where TEntity : class
{
    private readonly TContext _context;
    private readonly AsyncPolicy _retryPolicy;

    public ContactRepository(TContext context)
    {
        _context = context;
        _retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task AddAsync(TEntity entity)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        _context.Set<TEntity>().AddAsync(entity));
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        });
    }

    public async Task DeleteAsync(TEntity entity)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        });
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            return await _context.Set<TEntity>().FindAsync(id);
        });
    }
}
