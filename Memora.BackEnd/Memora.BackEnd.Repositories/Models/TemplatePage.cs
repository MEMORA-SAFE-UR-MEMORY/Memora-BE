using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class TemplatePage
{
    public long Id { get; set; }

    public long TemplateId { get; set; }

    public int PageNo { get; set; }

    public string? LayoutUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool HasSlots { get; set; }

    public virtual ICollection<AlbumPage> AlbumPages { get; set; } = new List<AlbumPage>();

    public virtual Template Template { get; set; } = null!;

    public virtual ICollection<TemplatePageSlot> TemplatePageSlots { get; set; } = new List<TemplatePageSlot>();
}
