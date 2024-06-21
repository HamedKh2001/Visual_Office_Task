using System;
using System.Collections.Generic;
using SharedKernel.Common;

namespace VO.Domain.Entities;

public class Order : BaseEntity
{
    public string Number { get; set; }
    public long TotalPrice { get; set; }
    public DateTime Date { get; set; }

    public ICollection<OrderLine> OrderLines { get; set; }
}