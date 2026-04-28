using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MNR.SDK.DataAccess.Context;
using MNR.SDK.DataAccess.Models;

namespace MNR.SDK.DataAccess.Repositories.Internals;

public abstract class BaseRepository<T, TId>(BaseContext context) : IBaseRepository<T, TId>
    where T : IdEntity<TId>
{
    protected BaseContext Context { get; } = context;
    protected abstract DbSet<T> Set { get; }
    
    public T Add(T entity)
    {
        Set.Add(entity);
        
        return entity;
    }

    public T Update(T entity)
    {
        Set.Update(entity);
        
        return entity;
    }

    public bool Remove(T entity)
    {
        Set.Remove(entity);
        return true;
    }

    public List<T> Add(List<T> entities)
    {
        Set.AddRange(entities);
        return entities;
    }

    public List<T> Update(List<T> entities)
    {
        Set.UpdateRange(entities);
        return entities;
    }

    public bool Remove(List<T> entities)
    {
        Set.RemoveRange(entities);
        return true;
    }

    public async Task<bool> Save(CancellationToken token = default)
        => await Context.SaveChangesAsync(token) != 0;

    public async Task<T?> AddAndSave(T entity, CancellationToken token = default)
    {
        Set.Add(entity);
        
        if (await Save(token))
            return entity;

        return null;
    }

    public async Task<T?> UpdateAndSave(T entity, CancellationToken token = default)
    {
        Set.Update(entity);
        
        if (await Save(token))
            return entity;

        return null;
    }

    public async Task<bool> RemoveAndSave(T entity, CancellationToken token = default)
    {
        Set.Remove(entity);
        
        return await Save(token);
    }

    public async Task<List<T>?> AddAndSave(List<T> entities, CancellationToken token = default)
    {
        Set.AddRange(entities);

        if (await Save(token))
            return entities;

        return null;
    }

    public async Task<List<T>?> UpdateAndSave(List<T> entities, CancellationToken token = default)
    {
        Set.UpdateRange(entities);

        if (await Save(token))
            return entities;

        return null;
    }

    public async Task<bool> RemoveAndSave(List<T> entities, CancellationToken token = default)
    {
        Set.UpdateRange(entities);
        return await Save(token);
    }

    public Task<TR?> Read<TR>(Expression<Func<T, bool>> expression, Expression<Func<T, TR>> selector,
        CancellationToken token = default)
        => Set
            .AsNoTracking()
            .AsSplitQuery()
            .Select(selector)
            .FirstOrDefaultAsync(token);

    public async Task<(List<TR> list, int total)> ReadList<TR>(Expression<Func<T, bool>> expression, Expression<Func<T, TR>> selector,
        int page = 1, int count = int.MaxValue,
        CancellationToken token = default)
    {
        var items = await Set
            .AsNoTracking()
            .AsSplitQuery()
            .Where(expression)
            .Select(selector)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync(token);
        var total = await Set.AsNoTracking().CountAsync(expression, token);

        return (items, total);
    }

    public Task<bool> Any(Expression<Func<T, bool>> expression, CancellationToken token = default)
        => Set.AnyAsync(expression, token);

    public Task<T?> Get(Expression<Func<T, bool>> expression, CancellationToken token = default,
        params Expression<Func<T, object>>[]? include)
    {
        IQueryable<T> query = Set;

        if (include != null && include.Any())
            query = include.Aggregate(query, (current, inc) => current.Include(inc));

        return query.FirstOrDefaultAsync(expression, token);
    }

    public Task<List<T>> GetList(Expression<Func<T, bool>> expression, CancellationToken token = default,
        params Expression<Func<T, object>>[]? include)
    {
        IQueryable<T> query = Set;

        if (include != null && include.Any())
            query = include.Aggregate(query, (current, inc) => current.Include(inc));

        return query.Where(expression)
            .ToListAsync(token);
    }
}