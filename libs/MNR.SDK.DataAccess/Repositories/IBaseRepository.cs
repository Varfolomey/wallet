using System.Linq.Expressions;
using MNR.SDK.DataAccess.Models;

namespace MNR.SDK.DataAccess.Repositories;

public interface IBaseRepository<T, TId> where T : IdEntity<TId>
{
    //base
    T Add(T entity);
    T Update(T entity);
    bool Remove(T entity);
    List<T> Add(List<T> entities);
    List<T> Update(List<T> entities);
    bool Remove(List<T> entities);
    Task<bool> Save(CancellationToken token = default);
    
    Task<T?> AddAndSave(T entity, CancellationToken token = default);
    Task<T?> UpdateAndSave(T entity, CancellationToken token = default);
    Task<bool> RemoveAndSave(T entity, CancellationToken token = default);
    Task<List<T>?> AddAndSave(List<T> entities, CancellationToken token = default);
    Task<List<T>?> UpdateAndSave(List<T> entities, CancellationToken token = default);
    Task<bool> RemoveAndSave(List<T> entities, CancellationToken token = default);
    
    //read
    Task<TR?> Read<TR>(Expression<Func<T, bool>> expression,
        Expression<Func<T, TR>> selector,
        CancellationToken token = default);
    Task<(List<TR> list, int total)> ReadList<TR>(Expression<Func<T, bool>> expression, 
        Expression<Func<T,TR>> selector, 
        int page = 1, 
        int count = int.MaxValue, 
        CancellationToken token = default);

    Task<bool> Any(Expression<Func<T, bool>> expression,
        CancellationToken token = default);
    
    //get
    Task<T?> Get(Expression<Func<T, bool>> expression, CancellationToken token = default,
        params Expression<Func<T, object>>[]? include);
    Task<List<T>> GetList(Expression<Func<T, bool>> expression, CancellationToken token = default,
        params Expression<Func<T, object>>[]? include);
}