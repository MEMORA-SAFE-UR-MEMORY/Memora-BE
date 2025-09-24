using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Memory
{
    public long Id { get; set; }

    public long RoomId { get; set; }

    public string? Title { get; set; }

    public string? FilePath { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateOnly Date { get; set; }

    public virtual Room Room { get; set; } = null!;
}
