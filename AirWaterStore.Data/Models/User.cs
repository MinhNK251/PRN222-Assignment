using System;
using System.Collections.Generic;

namespace AirWaterStore.Data.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public bool? IsBan { get; set; }

    public virtual ICollection<ChatRoom> ChatRoomCustomers { get; set; } = new List<ChatRoom>();

    public virtual ICollection<ChatRoom> ChatRoomStaffs { get; set; } = new List<ChatRoom>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
