using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Item
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal? PuzzlePrice { get; set; }

    public long CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string ItemImagePath { get; set; } = null!;

    public long? DimensionId { get; set; }

    public long? ThemeId { get; set; }

    public string Type { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ItemDimension? Dimension { get; set; }

    public virtual ICollection<FrameSlot> FrameSlots { get; set; } = new List<FrameSlot>();

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual ICollection<RoomItem> RoomItems { get; set; } = new List<RoomItem>();

    public virtual Theme? Theme { get; set; }
}
