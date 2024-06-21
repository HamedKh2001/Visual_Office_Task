using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VO.Application.Contracts.Persistence;
using VO.Domain.Entities;

namespace VO.Application.Features.Product.Queries.TotalSoldProducts;

public class TotalSoldProductsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<TotalSoldProductsQuery, int>
{
    public async Task<int> Handle(TotalSoldProductsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.GetRepository<OrderLine>(true).GetQueryable();

        var totalQuantity = await query
            .Where(o => o.Order.Date >= request.FromDate && o.Order.Date <= request.ToDate)
            .SumAsync(o => o.Quantity, cancellationToken);

        return totalQuantity;
    }
}