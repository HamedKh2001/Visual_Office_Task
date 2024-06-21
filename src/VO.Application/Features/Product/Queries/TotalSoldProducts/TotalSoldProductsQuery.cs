using System;
using MediatR;

namespace VO.Application.Features.Product.Queries.TotalSoldProducts;

public record TotalSoldProductsQuery(DateTime FromDate, DateTime ToDate) : IRequest<int>;