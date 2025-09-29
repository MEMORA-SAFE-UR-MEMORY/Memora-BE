using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Wallet
{
    public long Id { get; set; }

    public decimal Puzzles { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public DateTime? LastDailyClaimAt { get; set; }

    public virtual User User { get; set; } = null!;
}
