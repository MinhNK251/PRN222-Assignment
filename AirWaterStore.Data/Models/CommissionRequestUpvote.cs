using System;
using System.Collections.Generic;

namespace AirWaterStore.Data.Models;

public partial class CommissionRequestUpvote
{
    public int UpvoteId { get; set; }

    public int CommissionRequestId { get; set; }

    public int UserId { get; set; }

    public DateTime? UpvotedAt { get; set; }

    public virtual CommissionRequest CommissionRequest { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
