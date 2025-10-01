using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumPageSlot
{
    public long Id { get; set; }

    public long AlbumPageId { get; set; }

    public long? TemplateSlotId { get; set; }

    public int SlotIndex { get; set; }

    public decimal XPct { get; set; }

    public decimal YPct { get; set; }

    public decimal WPct { get; set; }

    public decimal HPct { get; set; }

    public decimal? RotationDeg { get; set; }

    public int? ZIndex { get; set; }

    public string? Shape { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AlbumPage AlbumPage { get; set; } = null!;

    public virtual ICollection<PagePhoto> PagePhotos { get; set; } = new List<PagePhoto>();

    public virtual TemplatePageSlot? TemplateSlot { get; set; }
}
