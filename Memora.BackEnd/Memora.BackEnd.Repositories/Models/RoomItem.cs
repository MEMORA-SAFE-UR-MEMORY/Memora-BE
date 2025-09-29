using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class RoomItem
{
    public long Id { get; set; }

    public long? PosX { get; set; }

    public long? PosY { get; set; }

    public long ZIndex { get; set; }

    public decimal Rotation { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long RoomId { get; set; }

    public long? ItemId { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<SlotMemory> SlotMemories { get; set; } = new List<SlotMemory>();
}
