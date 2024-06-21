using SharedKernel.Common;

namespace VO.Domain.Entities;

public class Product : BaseEntity
{
    public int Price { get; set; }
    public string Title { get; set; }
}