using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class VTemplatePageSlotCount
{
    public long? TemplateId { get; set; }

    public long? TemplatePageId { get; set; }

    public int? PageNo { get; set; }

    public int? SlotCount { get; set; }
}
