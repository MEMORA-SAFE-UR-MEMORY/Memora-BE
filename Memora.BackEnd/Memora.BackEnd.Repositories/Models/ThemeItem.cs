using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class ThemeItem
{
    public long ThemeId { get; set; }

    public long ItemId { get; set; }

    public long DoorId { get; set; }

    public virtual Door Door { get; set; } = null!;

    public virtual Item Item { get; set; } = null!;

    public virtual Theme Theme { get; set; } = null!;
}
