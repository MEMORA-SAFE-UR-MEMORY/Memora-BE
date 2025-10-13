using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Order
{
    public string Status { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Fullname { get; set; }

    public Guid Id { get; set; }

    public long? PayOsOrderCode { get; set; }

    public virtual ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();

    public virtual User User { get; set; } = null!;
}
