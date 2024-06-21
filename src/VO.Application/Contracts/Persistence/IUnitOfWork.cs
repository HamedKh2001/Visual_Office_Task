using System.Threading.Tasks;
using SharedKernel.Common;
using VO.Application.Common;
using VO.Application.Contracts.Persistence.Repository;

namespace VO.Application.Contracts.Persistence
{
    public interface IUnitOfWork
    {
        public IRepository<TEntity> GetRepository<TEntity>(bool isNoTracking = false) where TEntity : class, IEntity;
        Task<int> CommitAsync();
        ValueTask RollBackAsync();
    }
}
