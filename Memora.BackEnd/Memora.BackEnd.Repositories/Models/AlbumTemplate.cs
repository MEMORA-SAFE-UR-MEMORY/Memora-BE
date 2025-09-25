using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumTemplate
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public long PageNumber { get; set; }

    public string LayoutUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
