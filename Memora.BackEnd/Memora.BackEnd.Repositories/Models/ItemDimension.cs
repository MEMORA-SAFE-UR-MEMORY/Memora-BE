using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class ItemDimension
{
    public long Id { get; set; }

    public int W { get; set; }

    public int H { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
