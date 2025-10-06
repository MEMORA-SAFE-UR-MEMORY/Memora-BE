using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class TemplatePageSlotsWithPage
{
    public long? Id { get; set; }

    public long? TemplatePageId { get; set; }

    public int? SlotIndex { get; set; }

    public string? Shape { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? TemplateId { get; set; }

    public int? PageNo { get; set; }

    public string? LayoutUrl { get; set; }
}
