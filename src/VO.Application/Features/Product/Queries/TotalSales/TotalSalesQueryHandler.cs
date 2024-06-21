using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VO.Application.Contracts.Persistence;
using VO.Domain.Entities;

namespace VO.Application.Features.Product.Queries.TotalSales;

public class TotalSalesQueryHandler : IRequestHandler<TotalSalesQuery, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public TotalSalesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(TotalSalesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.GetRepository<Order>(true).GetQueryable();

        var totalSale = await query.Where(o => o.OrderLines.Count != 0)
            .Include(o => o.OrderLines.Where(ol => ol.Product.Id == request.ProductId))
            .ThenInclude(ol => ol.Product)
            .SumAsync(o => o.TotalPrice, cancellationToken);

        return totalSale;
    }
}