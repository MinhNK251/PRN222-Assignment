using System;
using System.Collections.Generic;

namespace AirWaterStore.Data.Models;

public partial class CommissionRequest
{
    public int CommissionRequestId { get; set; }

    public int UserId { get; set; }

    public string GameTitle { get; set; } = null!;

    public decimal ExpectedPrice { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public int? Upvotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CommissionRequestUpvote> CommissionRequestUpvotes { get; set; } = new List<CommissionRequestUpvote>();

    public virtual User User { get; set; } = null!;
}
