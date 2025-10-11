using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Album
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public long? TemplateId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsOrdered { get; set; }

    public virtual ICollection<AlbumPage> AlbumPages { get; set; } = new List<AlbumPage>();

    public virtual ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();

    public virtual Template? Template { get; set; }

    public virtual User User { get; set; } = null!;
}
