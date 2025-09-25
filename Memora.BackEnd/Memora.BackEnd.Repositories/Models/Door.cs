using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Door
{
    public long Id { get; set; }

    public string ImgUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string ColorHex { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual Theme? Theme { get; set; }
}
