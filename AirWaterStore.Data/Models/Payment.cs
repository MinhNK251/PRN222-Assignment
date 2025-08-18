using System;
using System.Collections.Generic;

namespace AirWaterStore.Data.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
