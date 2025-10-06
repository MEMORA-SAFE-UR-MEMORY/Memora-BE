using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumPageSlot
{
    public long Id { get; set; }

    public long AlbumPageId { get; set; }

    public long? TemplateSlotId { get; set; }

    public int SlotIndex { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AlbumPage AlbumPage { get; set; } = null!;

    public virtual TemplatePageSlot? TemplateSlot { get; set; }
}
