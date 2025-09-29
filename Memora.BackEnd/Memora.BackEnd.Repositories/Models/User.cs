using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }

    public short RoleId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Fullname { get; set; }

    public string? RefreshToken { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual Inventory? Inventory { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<UserEntitlement> UserEntitlements { get; set; } = new List<UserEntitlement>();

    public virtual ICollection<UserTheme> UserThemes { get; set; } = new List<UserTheme>();

    public virtual Wallet? Wallet { get; set; }
}
