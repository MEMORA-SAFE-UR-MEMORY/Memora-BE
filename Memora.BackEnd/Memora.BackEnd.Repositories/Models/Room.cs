using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Room
{
    public long Id { get; set; }

    public string RoomName { get; set; } = null!;

    public long? ThemeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public long DoorId { get; set; }

    public string Type { get; set; } = null!;

    public virtual Door Door { get; set; } = null!;

    public virtual ICollection<Memory> Memories { get; set; } = new List<Memory>();

    public virtual ICollection<RoomItem> RoomItems { get; set; } = new List<RoomItem>();

    public virtual Theme? Theme { get; set; }

    public virtual User User { get; set; } = null!;
}
