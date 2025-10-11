using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class OrderAlbum
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long Quantity { get; set; }

    public decimal Price { get; set; }

    public long AlbumId { get; set; }

    public Guid OrderId { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
