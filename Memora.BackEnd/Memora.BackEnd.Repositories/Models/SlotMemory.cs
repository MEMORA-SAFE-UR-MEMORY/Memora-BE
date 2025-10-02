using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class SlotMemory
{
    public long Id { get; set; }

    public long RoomItemId { get; set; }

    public long FrameSlotId { get; set; }

    public long MemoryId { get; set; }

    public virtual FrameSlot FrameSlot { get; set; } = null!;

    public virtual Memory Memory { get; set; } = null!;

    public virtual RoomItem RoomItem { get; set; } = null!;
}
