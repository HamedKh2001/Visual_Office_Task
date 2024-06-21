using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Common;
using VO.Application.Contracts.Persistence;
using VO.Application.Contracts.Persistence.Repository;
using VO.Infrastructure.Persistence;

namespace VO.Infrastructure.Repositories.Common;

public class UnitOfWork(ApplicationWriteDbContext writeDbContext, ApplicationReadDbContext readDbContext)
    : IUnitOfWork
{
    private readonly Dictionary<Type, object> _readRepositories = new();

    private readonly Dictionary<Type, object> _writeRepositories = new();

    public IRepository<TEntity> GetRepository<TEntity>(bool onlyRead = false) where TEntity : class, IEntity
    {
        Dictionary<Type, object> repositories = onlyRead ? _readRepositories : _writeRepositories;
        if (repositories.ContainsKey(typeof(TEntity)))
            return repositories[typeof(TEntity)] as Repository<TEntity>;

        var repository = new Repository<TEntity>(onlyRead ? readDbContext : writeDbContext, onlyRead);
        repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public Task<int> CommitAsync()
    {
        return writeDbContext.SaveChangesAsync();
    }

    public ValueTask RollBackAsync()
    {
        return writeDbContext.DisposeAsync();
    }
}