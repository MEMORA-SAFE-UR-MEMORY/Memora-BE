using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Theme
{
    public long Id { get; set; }

    public string ThemeName { get; set; } = null!;

    public decimal ThemePrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public string WallUrl { get; set; } = null!;

    public string FloorUrl { get; set; } = null!;

    public long? DoorId { get; set; }

    public string? RevenueCatProductId { get; set; }

    public virtual Door? Door { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<UserTheme> UserThemes { get; set; } = new List<UserTheme>();
}
