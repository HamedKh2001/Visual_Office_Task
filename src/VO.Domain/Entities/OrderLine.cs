using SharedKernel.Common;

namespace VO.Domain.Entities;

public class OrderLine : BaseEntity
{
    public int Quantity { get; set; }
    public int Price { get; set; }
    
    public Order Order { get; set; }
    public Product Product { get; set; }
}