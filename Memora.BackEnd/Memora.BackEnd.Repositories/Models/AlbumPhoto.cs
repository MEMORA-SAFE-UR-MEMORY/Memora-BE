using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumPhoto
{
    public long Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public long PageNumber { get; set; }

    public long Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? AlbumId { get; set; }

    public virtual Album? Album { get; set; }
}
