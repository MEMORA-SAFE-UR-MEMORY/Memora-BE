using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Inventory
{
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public long Id { get; set; }

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual User User { get; set; } = null!;
}
