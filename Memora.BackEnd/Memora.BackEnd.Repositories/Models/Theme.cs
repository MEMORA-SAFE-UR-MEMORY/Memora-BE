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

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<ThemeItem> ThemeItems { get; set; } = new List<ThemeItem>();
}
