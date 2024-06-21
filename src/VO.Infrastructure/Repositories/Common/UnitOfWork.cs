using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.Common;
using VO.Application.Contracts.Persistence;
using VO.Application.Contracts.Persistence.Repository;
using VO.Infrastructure.Persistence;

namespace VO.Infrastructure.Repositories.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<Type, object> _readRepositories = new();

    private readonly Dictionary<Type, object> _writeRepositories = new();
    private readonly ApplicationWriteDbContext _writeDbContext;
    private readonly ApplicationReadDbContext _readDbContext;

    public UnitOfWork(ApplicationWriteDbContext writeDbContext, ApplicationReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    public IRepository<TEntity> GetRepository<TEntity>(bool onlyRead = false) where TEntity : class, IEntity
    {
        Dictionary<Type, object> repositories = onlyRead ? _readRepositories : _writeRepositories;
        if (repositories.ContainsKey(typeof(TEntity)))
            return repositories[typeof(TEntity)] as Repository<TEntity>;

        var repository = new Repository<TEntity>(onlyRead ? _readDbContext : _writeDbContext, onlyRead);
        repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public Task<int> CommitAsync()
    {
        return _writeDbContext.SaveChangesAsync();
    }

    public ValueTask RollBackAsync()
    {
        return _writeDbContext.DisposeAsync();
    }
}