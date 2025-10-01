using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class TemplatePageSlot
{
    public long Id { get; set; }

    public long TemplatePageId { get; set; }

    public int SlotIndex { get; set; }

    public decimal XPct { get; set; }

    public decimal YPct { get; set; }

    public decimal WPct { get; set; }

    public decimal HPct { get; set; }

    public decimal? RotationDeg { get; set; }

    public int? ZIndex { get; set; }

    public string? Shape { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<AlbumPageSlot> AlbumPageSlots { get; set; } = new List<AlbumPageSlot>();

    public virtual TemplatePage TemplatePage { get; set; } = null!;
}
