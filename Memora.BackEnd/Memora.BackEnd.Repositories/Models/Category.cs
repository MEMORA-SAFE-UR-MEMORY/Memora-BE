using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Category
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
