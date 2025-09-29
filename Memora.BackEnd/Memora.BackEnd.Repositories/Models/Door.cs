using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Door
{
    public long Id { get; set; }

    public string ImgUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? ColorHex { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();
}
