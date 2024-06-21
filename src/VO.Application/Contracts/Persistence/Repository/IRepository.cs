using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Common;
using VO.Application.Common;

namespace VO.Application.Contracts.Persistence.Repository;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    public IModel Model { get; }
    Task AddNew(TEntity entity);
    Task MultiAddNew(TEntity[] entities);
    void SetEntityState(TEntity entity, EntityState state);
    Task<int> Count(Expression<Func<TEntity, bool>> where);
    Task<T> Max<T>(Expression<Func<TEntity, T>> max, Expression<Func<TEntity, bool>> where);
    Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> where);
    Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> where, List<string> includesList);

    Task<(List<TEntity>, int, int)> PaginatedQuery(List<string> includesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize, int pageNumber, Expression<Func<TEntity, dynamic>> orderBy,
        bool descending = false, bool runAsSingleQuery = false, Expression<Func<TEntity, bool>> where = null);

    Task<(List<TEntity>, int, int)> PaginatedQuery(Expression<Func<TEntity, bool>> where,
        List<string> includesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize,
        int pageNumber,
        Expression<Func<TEntity, object>> orderBy,
        bool descending = false,
        bool runAsSingleQuery = false);

    Task<(List<TEntity>, int, int)> PaginatedQuery(
        Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, object>>> includesList,
        List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> thenIncludesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize,
        int pageNumber,
        Expression<Func<TEntity, object>> orderBy,
        bool descending = false,
        bool runAsSingleQuery = false);

    Task<(List<TEntity>, int, int)> PaginatedQuery(Expression<Func<TEntity, bool>> where,
        List<Expression<Func<TEntity, object>>> includesList,
        Expression<Func<TEntity, bool>> filterExpression,
        Expression<Func<TEntity, bool>> searchExpression,
        int pageSize,
        int pageNumber,
        Expression<Func<TEntity, object>> orderBy,
        bool descending = false,
        bool runAsSingleQuery = false);


    Task<List<TEntity>> QueryWithOrderBy(Expression<Func<TEntity, bool>> where,
        Expression<Func<TEntity, dynamic>> orderBy, bool isDescendingOrder, int limit);

    Task<TEntity> FindOne(Expression<Func<TEntity, bool>> where, List<string> includesList = null);
    Task<TEntity> FindFirst(Expression<Func<TEntity, bool>> where, List<string> includesList = null);
    void Delete(TEntity entity);
    Task Update(TEntity entity);
    Task<int> BatchDelete(Expression<Func<TEntity, bool>> where);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where);

    Task<int> BatchUpdateImmediate(Expression<Func<TEntity, bool>> whereExpression,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> propertyCall);

    //void Include<TProperty>(Expression<Func<TEntity, TProperty>> include,
    //    List<Expression<Func<TProperty, TProperty>>> thenInclude);

    //void ResetIncludes();

    IQueryable<TEntity> GetQueryable();
    Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> where, List<Expression<Func<TEntity, object>>> includesList);

    Task<TEntity> FindFirstAsync(Expression<Func<TEntity, bool>> where, List<Expression<Func<TEntity, object>>> includesList);
    Task<TEntity> Last(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy,
        bool descending = false, List<string> includesList = null);
}