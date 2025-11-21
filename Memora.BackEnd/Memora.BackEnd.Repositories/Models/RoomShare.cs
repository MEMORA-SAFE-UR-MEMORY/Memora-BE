using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class RoomShare
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long RoomId { get; set; }

    public Guid InvitedUserId { get; set; }

    public Guid InvitedBy { get; set; }

    public virtual User InvitedByNavigation { get; set; } = null!;

    public virtual User InvitedUser { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
