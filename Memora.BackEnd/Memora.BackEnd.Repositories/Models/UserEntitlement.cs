using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class UserEntitlement
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string EntitlementIdentifier { get; set; } = null!;

    public string ProductIdentifier { get; set; } = null!;

    public DateTime PurchaseDateUtc { get; set; }

    public DateTime? ExpiresDateUtc { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
