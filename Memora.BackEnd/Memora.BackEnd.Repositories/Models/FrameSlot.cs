using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class FrameSlot
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal X { get; set; }

    public decimal Y { get; set; }

    public decimal W { get; set; }

    public decimal H { get; set; }

    public decimal? Rotation { get; set; }

    public string Shape { get; set; } = null!;

    public long ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual ICollection<SlotMemory> SlotMemories { get; set; } = new List<SlotMemory>();
}
