using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class VAlbumPageSlotCount
{
    public long? AlbumId { get; set; }

    public long? AlbumPageId { get; set; }

    public int? PageNo { get; set; }

    public int? SlotCount { get; set; }
}
