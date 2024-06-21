using MediatR;

namespace VO.Application.Features.Product.Queries.TotalSales;

public record TotalSalesQuery(int ProductId):IRequest<decimal>;