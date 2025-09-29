using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class SlotMemory
{
    public long Id { get; set; }

    public long RoomItemId { get; set; }

    public long SlotId { get; set; }

    public long MemoryId { get; set; }

    public virtual Memory Memory { get; set; } = null!;

    public virtual RoomItem RoomItem { get; set; } = null!;

    public virtual FrameSlot Slot { get; set; } = null!;
}
