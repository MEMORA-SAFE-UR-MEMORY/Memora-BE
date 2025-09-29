using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class RevenuecatWebhookLog
{
    public string EventId { get; set; } = null!;

    public string EventType { get; set; } = null!;

    public DateTime ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsProcessed { get; set; }

    public string? ProcessingNotes { get; set; }
}
