using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class PagePhoto
{
    public long Id { get; set; }

    public long AlbumPageId { get; set; }

    public long SlotId { get; set; }

    public string? PhotoUrl { get; set; }

    public string? FitMode { get; set; }

    public decimal? OffsetXPct { get; set; }

    public decimal? OffsetYPct { get; set; }

    public decimal? ScalePct { get; set; }

    public decimal? RotationDeg { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual AlbumPage AlbumPage { get; set; } = null!;

    public virtual AlbumPageSlot Slot { get; set; } = null!;
}
