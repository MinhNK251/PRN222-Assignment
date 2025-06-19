using System;
using System.Collections.Generic;

namespace AirWaterStore.Data.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int ChatRoomId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
