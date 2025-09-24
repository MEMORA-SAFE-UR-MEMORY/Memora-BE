using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Order
{
    public long Id { get; set; }

    public long Status { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();

    public virtual User User { get; set; } = null!;
}
