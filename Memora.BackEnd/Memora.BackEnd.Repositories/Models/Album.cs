using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Album
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long TemplateId { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<AlbumPhoto> AlbumPhotos { get; set; } = new List<AlbumPhoto>();

    public virtual ICollection<OrderAlbum> OrderAlbums { get; set; } = new List<OrderAlbum>();

    public virtual AlbumTemplate Template { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
