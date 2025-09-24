using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class InventoryItem
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long Quantity { get; set; }

    public long ItemId { get; set; }

    public long InventoryId { get; set; }

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;
}
