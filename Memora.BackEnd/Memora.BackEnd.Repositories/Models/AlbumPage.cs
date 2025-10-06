using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumPage
{
    public long Id { get; set; }

    public long AlbumId { get; set; }

    public int PageNo { get; set; }

    public string LayoutSnapshotUrl { get; set; } = null!;

    public long? PageTemplateId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual ICollection<AlbumPageSlot> AlbumPageSlots { get; set; } = new List<AlbumPageSlot>();

    public virtual TemplatePage? PageTemplate { get; set; }
}
