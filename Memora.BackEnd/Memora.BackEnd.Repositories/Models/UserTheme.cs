using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class UserTheme
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long ThemeId { get; set; }

    public Guid UserId { get; set; }

    public virtual Theme Theme { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
