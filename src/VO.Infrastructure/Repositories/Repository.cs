using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Common;
using VO.Application.Contracts.Persistence.Repository;
using VO.Infrastructure.Persistence;
using VO.Infrastructure.Repositories.Common;

namespace VO.Infrastructure.Repositories;

internal class Repository<TEntity> : BaseAsyncRepository<TEntity>, IRepository<TEntity> where TEntity : class, IEntity
{
    public Repository(ApplicationDbContext dbContext, bool onlyRead) : base(dbContext, onlyRead)
    {
    }

    public IModel Model => DbContext.Model;

    public async Task AddNew(TEntity entity)
    {
        await base.AddAsync(entity);
    }

    public void SetEntityState(TEntity entity, EntityState state)
    {
        DbContext.Entry(entity).State = state;
    }

    public async Task<int> Count(Expression<Func<TEntity, bool>> where)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);
        return await query.CountAsync();
    }

    public async Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> where)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);
        return await query.ToListAsync();
    }


    public async Task<(List<TEntity>, int, int)> PaginatedQuery(List<string> includesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize, int pageNumber, Expression<Func<TEntity, dynamic>> orderBy,
        bool descending = false, bool runAsSingleQuery = false, Expression<Func<TEntity, bool>> where = null)
    {
        if (pageSize > int.MaxValue - 1)
            pageSize = int.MaxValue - 1;
        IQueryable<TEntity> query = Table;
        query = query.Where(q => q.DeletedDate == null);

        if (where is not null)
            query = query.Where(where);

        if (searchExpression is not null)
            query = query.Where(searchExpression);
        if (filterExpression is not null)
            query = query.Where(filterExpression);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));

        if (orderBy != null)
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        if (pageSize == 0)
            pageSize = 20;
        int count = await query.CountAsync();
        int fullPageCount = count / pageSize;
        if (pageNumber == 0)
            query = query.Take(pageSize);

        else if (int.IsPositive(pageNumber))
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        else
            query = fullPageCount + pageNumber + 1 < 0
                ? query.Take(pageSize)
                : query.Skip((fullPageCount + pageNumber + 1) * pageSize).Take(pageSize);

        if (runAsSingleQuery)
            query = query.AsSingleQuery();
        return (await query.ToListAsync(),
            (int)Math.Max(fullPageCount, Math.Ceiling((double)count / pageSize)), count);
    }

    public async Task<(List<TEntity>, int, int)> PaginatedQuery(Expression<Func<TEntity, bool>> where,
        List<string> includesList, Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize, int pageNumber, Expression<Func<TEntity, object>> orderBy, bool descending = false,
        bool runAsSingleQuery = false)
    {
        if (pageSize > int.MaxValue - 1)
            pageSize = int.MaxValue - 1;
        IQueryable<TEntity> query = Table;
        if (where is not null)
            query = query.Where(where);
        if (searchExpression is not null)
            query = query.Where(searchExpression);
        if (filterExpression is not null)
            query = query.Where(filterExpression);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));

        if (orderBy != null) query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        if (pageSize == 0)
            pageSize = 20;
        int count = await query.CountAsync();
        int fullPageCount = count / pageSize;
        if (pageNumber == 0)
            query = query.Take(pageSize);

        else if (int.IsPositive(pageNumber))
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        else
            query = fullPageCount + pageNumber + 1 < 0
                ? query.Take(pageSize)
                : query.Skip((fullPageCount + pageNumber + 1) * pageSize).Take(pageSize);

        if (runAsSingleQuery)
            query = query.AsSingleQuery();

        return (await query.ToListAsync(),
            (int)Math.Max(fullPageCount, Math.Ceiling((double)count / pageSize)), count);
    }

    public async Task<(List<TEntity>, int, int)> PaginatedQuery(Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, object>>> includesList, Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize, int pageNumber, Expression<Func<TEntity, object>> orderBy, bool descending = false,
        bool runAsSingleQuery = false)
    {
        if (pageSize > int.MaxValue - 1)
            pageSize = int.MaxValue - 1;
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);
        if (searchExpression is not null)
            query = query.Where(searchExpression);
        if (filterExpression is not null)
            query = query.Where(filterExpression);

        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));

        if (orderBy != null) query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        if (pageSize == 0)
            pageSize = 20;
        int count = await query.CountAsync();
        int fullPageCount = count / pageSize;
        if (pageNumber == 0)
            query = query.Take(pageSize);

        else if (int.IsPositive(pageNumber))
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        else
            query = fullPageCount + pageNumber + 1 < 0
                ? query.Take(pageSize)
                : query.Skip((fullPageCount + pageNumber + 1) * pageSize).Take(pageSize);

        if (runAsSingleQuery)
            query = query.AsSingleQuery();

        return (await query.ToListAsync(),
            (int)Math.Max(fullPageCount, Math.Ceiling((double)count / pageSize)), count);
    }

    public async Task<(List<TEntity>, int, int)> PaginatedQuery(
        Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, object>>> includesList,
        List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> thenIncludesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize,
        int pageNumber,
        Expression<Func<TEntity, object>> orderBy,
        bool descending = false,
        bool runAsSingleQuery = false)
    {
        if (pageSize > int.MaxValue - 1)
            pageSize = int.MaxValue - 1;

        IQueryable<TEntity> query = Table;

        if (where != null)
            query = query.Where(where);
        
        if (searchExpression != null)
            query = query.Where(searchExpression);

        if (filterExpression != null)
            query = query.Where(filterExpression);

        if (includesList != null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));

        // Apply ThenInclude operations
        if (thenIncludesList != null)
            query = thenIncludesList.Aggregate(query, (current, thenInclude) => thenInclude(current));

        if (orderBy != null)
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        if (pageSize == 0)
            pageSize = 20;

        int count = await query.CountAsync();
        int fullPageCount = count / pageSize;

        if (pageNumber == 0)
            query = query.Take(pageSize);
        else if (int.IsPositive(pageNumber))
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        else
            query = fullPageCount + pageNumber + 1 < 0
                ? query.Take(pageSize)
                : query.Skip((fullPageCount + pageNumber + 1) * pageSize).Take(pageSize);

        if (runAsSingleQuery)
            query = query.AsSingleQuery();

        return (await query.ToListAsync(),
            (int)Math.Max(fullPageCount, Math.Ceiling((double)count / pageSize)), count);
    }




    
    public async Task<TEntity> FindOne(Expression<Func<TEntity, bool>> where, List<string> includesList = null)
    {
        IQueryable<TEntity> query = Table.Where(where);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstAsync();
    }

    public async Task<TEntity> FindFirst(Expression<Func<TEntity, bool>> where, List<string> includesList = null)
    {
        IQueryable<TEntity> query = Table;
        if (where is not null)
            query = query.Where(where);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync();
    }

    public void Delete(TEntity entity)
    {
        DbContext.Set<TEntity>().Remove(entity);
    }

    public async Task Update(TEntity entity)
    {
        await base.TrackableUpdateAsync(entity);
    }

    public async Task MultiAddNew(TEntity[] entities)
    {
        await base.MultiAddAsync(entities);
    }

    public async Task<List<TEntity>> Query(
        Expression<Func<TEntity, bool>> where,
        List<string> includesList)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);
        query = includesList.Aggregate(query, (current, include) => current.Include(include));
        return await query.ToListAsync();
    }

    public async Task<List<TEntity>> QueryWithOrderBy(
        Expression<Func<TEntity, bool>> where,
        Expression<Func<TEntity, dynamic>> orderBy,
        bool isDescendingOrder,
        int limit)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);

        if (orderBy != null)
            query = isDescendingOrder ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        query = query.Take(limit);

        return await query.ToListAsync();
    }

    public async Task<int> BatchDelete(Expression<Func<TEntity, bool>> where)
    {
        return await Table.Where(where).ExecuteDeleteAsync();
    }

    public async Task<T> Max<T>(Expression<Func<TEntity, T>> max, Expression<Func<TEntity, bool>> where)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);
        return await query.MaxAsync(max);
    }

    public async Task<int> BatchUpdateImmediate(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> propertyCall)
    {
        IQueryable<TEntity> table = Table;
        if (whereExpression is not null)
            table = table.Where(whereExpression);
        return await table.ExecuteUpdateAsync(propertyCall);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where)
    {
        return await Table.AnyAsync(where);
    }

    //public void Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> include,
    //    List<Expression<Func<TProperty, TProperty>>> thenInclude)
    //{
    //    mainQuery ??= Table;
    //    var includableQueryable = mainQuery.Include(include);
    //    foreach (Expression<Func<TProperty, TProperty>> expression in thenInclude)
    //    {
    //        includableQueryable.ThenInclude(expression);
    //    }
    //    includableQueryable.ThenInclude()
    //    if (thenInclude is not null)
    //        includableQueryable = thenInclude.Aggregate(includableQueryable,
    //            (current, expression) => current.ThenInclude(expression));

    //    mainQuery = includableQueryable;
    //}

    //public void ResetIncludes()
    //{
    //    mainQuery = null;
    //}

    public IQueryable<TEntity> GetQueryable()
    {
        return Table;
    }

    public async Task<List<TEntity>> Query(
        Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, object>>> includesList)
    {
        IQueryable<TEntity> query = Table;
        if (where != null)
            query = query.Where(where);

        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));

        return await query.ToListAsync();
    }

    public async Task<TEntity> FindFirstAsync(Expression<Func<TEntity, bool>> where, List<Expression<Func<TEntity, object>>> includesList)
    {
        IQueryable<TEntity> query = Table;
        if (where is not null)
            query = query.Where(where);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));
        return await query.FirstOrDefaultAsync();
    }

    public async Task<TEntity> Last(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool descending = false, List<string> includesList = null)
    {
        IQueryable<TEntity> query = Table;
        if (orderBy != null) query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        if (where is not null)
            query = query.Where(where);
        if (includesList is not null)
            query = includesList.Aggregate(query, (current, include) => current.Include(include));
        return await query.LastOrDefaultAsync();
    }
}