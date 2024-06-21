using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VO.Application.Features.Product.Queries.TotalSales;
using VO.Application.Features.Product.Queries.TotalSoldProducts;

namespace VO.WebAPI.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ApiControllerBase
    {
        [HttpPost("{id}/total_sell")]
        public async Task<ActionResult<decimal>> TotalSales(int productId, CancellationToken cancellationToken)
        {
            return await Mediator.Send(new TotalSalesQuery(productId), cancellationToken);
        }

        [HttpPost("total_sold_products")]
        public async Task<ActionResult<decimal>> TotalSoldProducts([FromRoute] TotalSoldProductsQuery query,
            CancellationToken cancellationToken)
        {
            return await Mediator.Send(query, cancellationToken);
        }
    }
}